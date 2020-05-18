using KY.KYV.JobReader;
using KY.KYV.Topology.Algorithm;
using KY.KYV.Topology.Geometries;
using KY.KYV.Topology.Triangulate;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Geometry = KY.KYV.Topology.Geometries.Geometry;
using Point = KY.KYV.Topology.Geometries.Point;
using Polygon = System.Windows.Shapes.Polygon;

namespace KY.KYV.WPF.JobFov
{
    public enum DrawType
    {
        BOUNDINGBOX,
        CONVEX,
        TRIANGULATION,
        COMBINE_INTERSECTION,
        ENVELOP_INTERSECTION
    }

    public struct ClusterShape
    {
        public byte B;
        public byte G;
        public byte R;
        public Geometry Shape;

        public ClusterShape(byte _R, byte _G, byte _B, Geometry geometry)
        {
            R = _R; G = _G; B = _B; Shape = geometry;
        }
    }

    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables

        private byte alphaRGB = 0x00;
        private JobReader.Board boardInfo = null;
        private Dictionary<int, Dictionary<int, Point>> boardPoints = null;
        private Dictionary<int, Geometry> clusterConvexPol = null;
        private List<RectangleF> compBoundarys = null;
        private DrawType drawType = DrawType.COMBINE_INTERSECTION;
        private GeometryFactory geometryFactory = null;
        private bool isDragged = false;
        private bool isRedraw = false;
        private Line line1 = null;
        private Line line2 = null;
        private List<int> listDrawed = null;
        private bool loadedBoard = false;
        private BitmapImage PCBImage = null;
        private PointF pLast = new PointF();
        private string rootPath = string.Empty;
        private string selectedItem = null;
        private List<ClusterShape> shapeClusters;
        private bool ViewModeNavi = false;

        #endregion Variables

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            byte.TryParse(ConfigurationManager.AppSettings["alphaRGB"], out alphaRGB);
            rootPath = System.AppDomain.CurrentDomain.BaseDirectory;
            geometryFactory = new GeometryFactory();
            compBoundarys = new List<RectangleF>();
            listDrawed = new List<int>();
            shapeClusters = new List<ClusterShape>();
        }

        #endregion Constructors

        #region Events

        private void btnOpenJob_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                loadedBoard = false;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                PCBImage = null;
                openFileDialog.Filter = "job files (*.kyjob)|*.kyjob|All files (*.*)|*.*";
                string jobFilePath = null;
                if (openFileDialog.ShowDialog() == true)
                {
                    jobFilePath = openFileDialog.FileName;
                }
                if (string.IsNullOrWhiteSpace(jobFilePath))
                    return;
                int idx = openFileDialog.FileName.LastIndexOf("\\");

                //Convert board data
                DoEvents();
                prgStatus.Value = 10;
                txtProccess.Text = "Openning job file..";
                using (JobReader.Reader reader = new JobReader.Reader())
                {
                    try
                    {
                        reader.OpenFile(jobFilePath);
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(rootPath + "\\log.txt", ConfigurationManager.AppSettings["101"] + ConfigurationManager.AppSettings["Detail"] + ex.Message + "\n");
                        MessageBox.Show(ConfigurationManager.AppSettings["101"], "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    prgStatus.Value = 50;

                    try
                    {
                        boardInfo = JobReader.Reader.ConvertBoardData(reader);
                    }
                    catch (Exception ex)
                    {
                        File.AppendAllText(rootPath + "\\log.txt", ConfigurationManager.AppSettings["102"] + ConfigurationManager.AppSettings["Detail"] + ex.Message + "\n");
                        MessageBox.Show(ConfigurationManager.AppSettings["102"], "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                prgStatus.Value = 100;
                List<string> compList = new List<string>();

                //add component list name
                foreach (var comp in boardInfo.Components)
                {
                    compList.Add(comp.ArrayIndex + " " + comp.CRD);
                }
                CompList.SelectedItem = null;
                CompList.ItemsSource = compList;

                if (CompList.Items.Count > 0)
                {
                    loadedBoard = false;
                    CompList.SelectedItem = CompList.Items[0];
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(rootPath + "\\log.txt", ConfigurationManager.AppSettings["100"] + ConfigurationManager.AppSettings["Detail"] + ex.Message + "\n");
            }
        }

        private void btnTypeToogle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModeNavi = !ViewModeNavi;
                loadedBoard = false;
                UpdateDraw();
            }
            catch (Exception ex)
            {
                File.AppendAllText(rootPath + "\\log.txt", ConfigurationManager.AppSettings["103"] + ConfigurationManager.AppSettings["Detail"] + ex.Message + "\n");
            }
        }

        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double zoomScale = 0x00;
            double.TryParse(ConfigurationManager.AppSettings["ZoomScale"], out zoomScale);
            if (e.Delta > 0)
            {
                st.ScaleX *= zoomScale;
                st.ScaleY *= zoomScale;
            }
            else
            {
                st.ScaleX /= zoomScale;
                st.ScaleY /= zoomScale;
            }
        }

        private void CompList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //System.Diagnostics.Debug.WriteLine(e);
                if (e.AddedItems.Count > 0)
                {
                    if (e.AddedItems[0] is string s)
                    {
                        selectedItem = s;
                    }
                }
                UpdateDraw();
            }
            catch (Exception ex)
            {
                File.AppendAllText(rootPath + "\\log.txt", ConfigurationManager.AppSettings["105"] + ConfigurationManager.AppSettings["Detail"] + ex.Message + "\n");
            }
        }

        private void DoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Render,
                new Action(delegate
                {
                }));
        }

        private void ImageView_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                var screenLocation = e.GetPosition(ImageView);
                pLast = new PointF((float)screenLocation.X, (float)screenLocation.Y);
                var filterComps = compBoundarys.Where(c => c.Contains(pLast));

                if (filterComps.Any())
                {
                    var comp = filterComps.First();
                    int lineLength = 1000;
                    int.TryParse(ConfigurationManager.AppSettings["PickLengthCOM"], out lineLength);
                    DrawPickLine(lineLength, new PointF(comp.X, comp.Y), comp.Width, comp.Height);
                }
                isDragged = true;
            }
            catch (Exception ex)
            {
                File.AppendAllText(rootPath + "\\log.txt", ex.Message + "\n");
            }
        }

        private void ImageView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                isDragged = false;
            }
            catch (Exception ex)
            {
                File.AppendAllText(rootPath + "\\log.txt", ex.Message + "\n");
            }
        }

        private void ImageView_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragged == false)
                return;
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    var screenLocation = e.GetPosition(ImageView);
                    var pos = new PointF((float)screenLocation.X, (float)screenLocation.Y);
                    var matrix = mt.Matrix; // it's a struct
                    matrix.Translate(pos.X - pLast.X, pos.Y - pLast.Y);
                    mt.Matrix = matrix;
                    pLast = pos;
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(rootPath + "\\log.txt", ex.Message + "\n");
            }
        }

        private void rdoBBOX_Click(object sender, RoutedEventArgs e)
        {
            ResetDrawType();
        }

        private void RedrawMap_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                isRedraw = true;
                loadedBoard = false;
                UpdateDraw();
                isRedraw = false;
            }
            catch (Exception ex)
            {
                File.AppendAllText(rootPath + "\\log.txt", ConfigurationManager.AppSettings["104"] + ConfigurationManager.AppSettings["Detail"] + ex.Message + "\n");
            }
        }

        #endregion Events

        #region Drawing

        private void DrawBoundary(byte cr, byte cg, byte cb, List<System.Windows.Point> points)
        {
            double thickness = 0x00;
            double.TryParse(ConfigurationManager.AppSettings["BoundartThickness"], out thickness);
            Polygon polygon = new Polygon();
            polygon.StrokeThickness = thickness;
            polygon.Points = new PointCollection(points);
            polygon.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromArgb(alphaRGB, cr, cg, cb));
            polygon.StrokeDashArray = new DoubleCollection((new double[] { 2, 4 }).AsEnumerable());
            polygon.Uid = "Boundary" + Guid.NewGuid().ToString();
            ImageView.Children.Add(polygon);
        }

        private void DrawCluster()
        {
            ChoiceDrawType();

            //clear old boundary
            List<UIElement> elements = new List<UIElement>();
            foreach (UIElement elem in ImageView.Children)
                if (elem.Uid.Contains("Boundary"))
                    elements.Add(elem);
            foreach (var el in elements)
                ImageView.Children.Remove(el);

            //Draw boundary cluster
            try
            {
                shapeClusters.Clear();
                foreach (var cluster in boardPoints)
                {
                    int arrIdx = cluster.Key;
                    byte cr, cg, cb;
                    GetColorArray(arrIdx, out cr, out cg, out cb);
                    switch (drawType)
                    {
                        case DrawType.BOUNDINGBOX:
                            DrawClusterByBoudingBox(cluster.Value.Values.ToList(), cr, cg, cb);
                            break;

                        case DrawType.CONVEX:
                            DrawClusterByConvexHull(clusterConvexPol[arrIdx], cr, cg, cb);
                            break;

                        case DrawType.ENVELOP_INTERSECTION:
                        case DrawType.TRIANGULATION:
                        case DrawType.COMBINE_INTERSECTION:
                            DrawClusterByIntersectionArea(cr, cg, cb, arrIdx);
                            break;

                        default:
                            break;
                    }
                    int countArrIdx = boardInfo.Components.Select(com => com.ArrayIndex).Distinct().ToList().Count;
                    DoEvents();
                    prgStatus.Value = prgStatus.Value >= 100 ? 0 : prgStatus.Value + 100 * arrIdx / countArrIdx;
                    txtProccess.Text = arrIdx == countArrIdx ? "Done." : "Drawing board cluster map..";
                }

                listDrawed.Clear();
                for (int i = 0; i < shapeClusters.Count; i++)
                {
                    DrawBoundary(shapeClusters[i].R, shapeClusters[i].G, shapeClusters[i].B,
                        shapeClusters[i].Shape.Coordinates.ToList().ConvertAll(PointCoorToPointWindow));
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(rootPath + "\\log.txt", ConfigurationManager.AppSettings["106"] + ConfigurationManager.AppSettings["Detail"] + ex.Message + "\n");
            }
        }

        private void DrawClusterByBoudingBox(List<Point> clusterArray, byte cr, byte cg, byte cb)
        {
            var minX = clusterArray.Min(p => p.X);
            var minY = clusterArray.Min(p => p.Y);
            var maxX = clusterArray.Max(p => p.X);
            var maxY = clusterArray.Max(p => p.Y);
            System.Windows.Shapes.Rectangle r = new System.Windows.Shapes.Rectangle();
            r.Width = Math.Abs(maxX - minX);
            r.Height = Math.Abs(maxY - minY);
            r.StrokeThickness = 8;
            r.StrokeDashArray = new DoubleCollection((new double[] { 2, 4 }).AsEnumerable());
            r.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromArgb(alphaRGB, cr, cg, cb));
            r.Uid = "Boundary" + Guid.NewGuid().ToString();
            Canvas.SetLeft(r, minX);
            Canvas.SetTop(r, minY);
            ImageView.Children.Add(r);
        }

        private void DrawClusterByConvexHull(Geometry geometry, byte cr, byte cg, byte cb)
        {
            DrawBoundary(cr, cg, cb, geometry.Coordinates.ToList().ConvertAll(PointCoorToPointWindow));
        }

        private void DrawClusterByIntersectionArea(byte cr, byte cg, byte cb, int clusterId)
        {
            if (listDrawed.Contains(clusterId)) return;

            //find convex hull intersect
            int intersectPolIdx;
            Geometry geoCurrent, geoNext, geoIntersect;
            //Geometry unionpoly1 = null, unionpoly2 = null;
            FindClusterIntersect(clusterId, out intersectPolIdx, out geoCurrent, out geoNext, out geoIntersect);

            if (intersectPolIdx == -1)
            {
                GetColorArray(clusterId, out cr, out cg, out cb);
                //DrawBoundary(cr, cg, cb, geoCurrent.Coordinates.ToList().ConvertAll(PointCoorToPointWindow));
                shapeClusters.Add(new ClusterShape(cr, cg, cb, geoCurrent));
                listDrawed.Add(clusterId);
                return;
            }

            //Reduce triangle
            //DoEvents();
            //txtProccess.Text = "Reduce intersection triangles or combine intersection area board array..";
            //prgStatus.Value += prgStatus.Value == 100 ? 0 : 10;
            if (drawType == DrawType.TRIANGULATION)
                ReduceByIntersectionTriangle(clusterId, intersectPolIdx, geoIntersect);
            else if (drawType == DrawType.COMBINE_INTERSECTION)
                CombineIntersectionArea(clusterId, intersectPolIdx, geoIntersect);
            else if (drawType == DrawType.ENVELOP_INTERSECTION)
                ReduceByEnvelopIntersection(clusterId, intersectPolIdx, geoIntersect);

            //DoEvents();
            //txtProccess.Text = "Drawing board arrays: " + clusterId + " and " + intersectPolIdx + "..";
            //prgStatus.Value += prgStatus.Value == 100 ? 0 : 10;
            //if (unionpoly1 != null)
            //{
            //    GetColorArray(clusterId, out cr, out cg, out cb);
            //    DrawBoundary(cr, cg, cb, unionpoly1.Coordinates.ToList().ConvertAll(PointCoorToPointWindow));
            //}
            //if (unionpoly2 != null)
            //{
            //    GetColorArray(intersectPolIdx, out cr, out cg, out cb);
            //    DrawBoundary(cr, cg, cb, unionpoly2.Coordinates.ToList().ConvertAll(PointCoorToPointWindow));
            //}
            listDrawed.Add(clusterId);
            listDrawed.Add(intersectPolIdx);
        }

        private void DrawFov(object item)
        {
            if (item is string s)
            {
                string[] sInfo = s.Split(' ');
                if (sInfo.Length != 2) return;
                int array = 0;
                if (!int.TryParse(sInfo[0], out array)) return;

                var comps = from JobReader.Component comp in boardInfo.Components
                            where (string.Compare(comp.CRD, sInfo[1]) == 0) && (comp.ArrayIndex == array)
                            select comp;
                foreach (JobReader.Component comp in comps)
                {
                    PointF compPoint = new PointF(comp.Location.X, comp.Location.Y);

                    foreach (JobReader.ImageData fovimage in boardInfo.FovImages)
                    {
                        RectangleF rectF = new RectangleF(fovimage.Location, fovimage.Size);
                        bool ret = rectF.Contains(compPoint);
                        if (ret)
                        {
                            if (loadedBoard == false && isRedraw == false)
                            {
                                ImageView.Children.Clear();
                            }
                            //System.Diagnostics.Debug.WriteLine(fovimage.ImgFilePath);
                            System.Windows.Controls.Image goodFovImage = new System.Windows.Controls.Image();
                            goodFovImage.Source = LoadImageFile(fovimage.ImgFilePath);

                            #region Add Extra FOV

                            //
                            //    +------------+------------+
                            //    |1           |2           |
                            //    |   x        |       x    |
                            //    |            |            |
                            //    +------------+------------+
                            //    |3         * |4           |
                            //    |            |            |
                            //    |   x        |       x    |
                            //    +------------+------------+
                            PointF[] compPointExtends = new PointF[]
                            {
                                    new PointF(compPoint.X-(float)fovimage.Size.Width /2,compPoint.Y-(float)fovimage.Size.Height /2),
                                    new PointF(compPoint.X,compPoint.Y-(float)fovimage.Size.Height /2),
                                    new PointF(compPoint.X+(float)fovimage.Size.Width /2,compPoint.Y-(float)fovimage.Size.Height /2),

                                    new PointF(compPoint.X-(float)fovimage.Size.Width /2,compPoint.Y),
                                    new PointF(compPoint.X,compPoint.Y),
                                    new PointF(compPoint.X+(float)fovimage.Size.Width /2,compPoint.Y),

                                    new PointF(compPoint.X-(float)fovimage.Size.Width /2,compPoint.Y+(float)fovimage.Size.Height /2),
                                    new PointF(compPoint.X,compPoint.Y+(float)fovimage.Size.Height /2),
                                    new PointF(compPoint.X+(float)fovimage.Size.Width /2,compPoint.Y+(float)fovimage.Size.Height /2),
                            };

                            PointF basePoint = new PointF();
                            for (int i = 0; i < compPointExtends.Length; i++)
                            {
                                foreach (JobReader.ImageData extrafovimage in boardInfo.FovImages)
                                {
                                    RectangleF rectExtraF = new RectangleF(extrafovimage.Location, extrafovimage.Size);
                                    if (rectExtraF.Contains(compPointExtends[i]))
                                    {
                                        //System.Diagnostics.Debug.WriteLine(compPointExtends[i] + ":" + rectExtraF + ":" + extrafovimage.ImgFilePath);
                                        if (i == 0)
                                            basePoint = extrafovimage.Location;
                                        System.Windows.Controls.Image fovImage = new System.Windows.Controls.Image();
                                        fovImage.Source = LoadImageFile(extrafovimage.ImgFilePath);

                                        float ratioEX = 13;
                                        float ratioEY = 13;
                                        if (fovImage.Source != null)
                                        {
                                            ratioEX = (float)fovImage.Source.Width / extrafovimage.Size.Width;
                                            ratioEY = (float)fovImage.Source.Height / extrafovimage.Size.Height;
                                        }
                                        PointF offset = new PointF(
                                            (extrafovimage.Location.X - basePoint.X) * ratioEX,
                                            (extrafovimage.Location.Y - basePoint.Y) * ratioEY
                                            );
                                        Canvas.SetLeft(fovImage, offset.X);
                                        Canvas.SetTop(fovImage, offset.Y);
                                        ImageView.Children.Add(fovImage);
                                        break;
                                    }
                                }
                            }

                            #endregion Add Extra FOV

                            //    ImageView.Children.Add(goodFovImage);
                            //goodFovImage.Stretch = Stretch.Uniform;
                            float ratioX = 13;
                            float ratioY = 13;

                            if (goodFovImage.Source != null)
                            {
                                ratioX = (float)goodFovImage.Source.Width / fovimage.Size.Width;
                                ratioY = (float)goodFovImage.Source.Height / fovimage.Size.Height;
                            }

                            PointF goodImageFovCenter = new PointF(
                                (compPoint.X - basePoint.X) * ratioX
                                , (compPoint.Y - basePoint.Y) * ratioY);

                            int lineLength = 60;
                            int.TryParse(ConfigurationManager.AppSettings["PickLengthFOV"], out lineLength);
                            DrawPickLine(lineLength, goodImageFovCenter);
                        }
                    }
                }
            }
        }

        private void DrawMap(object item)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            if (item is string s)
            {
                //get (array index + name) of comp
                string[] sInfo = s.Split(' ');
                if (sInfo.Length != 2) return;
                int array = 0;
                if (!int.TryParse(sInfo[0], out array)) return;

                //only clear all objects at first time
                if (loadedBoard == false && isRedraw == false)
                    ImageView.Children.Clear();

                #region Cal ratio to fit comp to board image

                System.Windows.Controls.Image wholeImage = new System.Windows.Controls.Image();
                if (PCBImage == null && boardInfo.WholeImage.ImgFilePath != null)
                    PCBImage = LoadImageFile(boardInfo.WholeImage.ImgFilePath);

                wholeImage.Source = PCBImage;
                float ratioX = 30;
                float ratioY = 30;

                if (wholeImage.Source != null)
                {
                    if (loadedBoard == false) ImageView.Children.Add(wholeImage);
                    ratioX = (float)wholeImage.Source.Width / boardInfo.Size.Width;
                    ratioY = (float)wholeImage.Source.Height / boardInfo.Size.Height;
                }

                #endregion Cal ratio to fit comp to board image

                int clusterArrayId = 0;
                Dictionary<int, Point> clusterArray = new Dictionary<int, Point>();
                boardPoints = new Dictionary<int, Dictionary<int, Point>>();
                clusterConvexPol = new Dictionary<int, Geometry>();

                int compId = 0;
                foreach (JobReader.Component comp in boardInfo.Components)
                {
                    byte cr = 0x00;
                    byte cg = 0x00;
                    byte cb = 0x00;
                    double originX = 0x00, originY = 0x00, comThickness = 0x00;
                    double.TryParse(ConfigurationManager.AppSettings["Ox"], out originX);
                    double.TryParse(ConfigurationManager.AppSettings["Oy"], out originY);
                    double.TryParse(ConfigurationManager.AppSettings["CompThickness"], out comThickness);
                    //init data to draw cluster boundary
                    if (loadedBoard == false && clusterArrayId != comp.ArrayIndex)
                    {
                        if (clusterArray.Count > 0)
                        {
                            boardPoints.Add(clusterArrayId, new Dictionary<int, Point>(clusterArray));
                            //Create Convex Polygon
                            var hullPol = ConvexHull.Create(clusterArray.Values.ToList());
                            clusterConvexPol.Add(clusterArrayId, hullPol);
                            clusterArray.Clear();
                        }
                        clusterArrayId = comp.ArrayIndex;
                    }

                    //init data component rect
                    if (loadedBoard == false) compId = InitCompRectList(ratioX, ratioY, clusterArray, compId, comp);

                    foreach (var drawData in comp.DrawDatas)
                    {
                        //Draw component
                        RectangleF rect = RatioRect(drawData.AbsLocation, drawData.Size, ratioX, ratioY);
                        if (loadedBoard == false && isRedraw == false)
                        {
                            System.Windows.Shapes.Rectangle r = new System.Windows.Shapes.Rectangle();
                            r.Width = rect.Width;
                            r.Height = rect.Height;
                            GetColorArray(comp.ArrayIndex, out cr, out cg, out cb, drawData.Type);
                            r.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromArgb(alphaRGB, cr, cg, cb));
                            r.StrokeThickness = 5;
                            Canvas.SetLeft(r, rect.X);
                            Canvas.SetTop(r, rect.Y);
                            r.RenderTransformOrigin = new System.Windows.Point(originX, originY);
                            RotateTransform rotateTransform1 = new RotateTransform(-drawData.AbsRoate);
                            r.RenderTransform = rotateTransform1;
                            ImageView.Children.Add(r);
                        }

                        //Draw picker
                        if (drawData.Type == JobReader.DrawData.DataType.BODY &&
                        string.Compare(comp.CRD, sInfo[1]) == 0 &&
                        array == comp.ArrayIndex)
                        {
                            int lineLength = 1000;
                            int.TryParse(ConfigurationManager.AppSettings["PickLengthCOM"], out lineLength);
                            DrawPickLine(lineLength, new PointF(rect.X, rect.Y), rect.Width, rect.Height);
                        }
                    }
                }

                //Draw boundary and clear all data
                if (loadedBoard == false && clusterArray.Count > 0)
                {
                    boardPoints.Add(clusterArrayId, new Dictionary<int, Point>(clusterArray));
                    var hullPol = ConvexHull.Create(clusterArray.Values.ToList());
                    clusterConvexPol.Add(clusterArrayId, hullPol);
                    DrawCluster();

                    boardPoints.Clear();
                    clusterArray.Clear();
                    clusterConvexPol.Clear();
                }
                prgStatus.Value = 100;
                txtProccess.Text = "Done.";
                stopWatch.Stop();
                if (loadedBoard == false)
                    txtProccess.Text = "Done. Time of proccess: " + stopWatch.ElapsedMilliseconds + " ms.";
                loadedBoard = true;
            }
        }

        private void DrawPickLine(int lineLength, PointF pointF, double compWidth = 0, double compHeight = 0)
        {
            byte cr = 0x00; byte cg = 0x00; byte cb = 0x00;
            byte.TryParse(ConfigurationManager.AppSettings["crPick"], out cr);
            byte.TryParse(ConfigurationManager.AppSettings["cgPick"], out cg);
            byte.TryParse(ConfigurationManager.AppSettings["cbPick"], out cb);
            {
                if (line1 != null) ImageView.Children.Remove(line1);
                line1 = new Line();
                line1.X1 = pointF.X + compWidth / 2 - lineLength;
                line1.Y1 = pointF.Y + compHeight / 2;
                line1.X2 = line1.X1 + lineLength + lineLength;
                line1.Y2 = line1.Y1;
                line1.StrokeThickness = 5;
                line1.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromArgb(alphaRGB, cr, cg, cb));
                ImageView.Children.Add(line1);
            }
            {
                if (line2 != null) ImageView.Children.Remove(line2);
                line2 = new Line();
                line2.X1 = pointF.X + compWidth / 2;
                line2.Y1 = pointF.Y + compHeight / 2 - lineLength;
                line2.X2 = line2.X1;
                line2.Y2 = line2.Y1 + lineLength + lineLength;
                line2.StrokeThickness = 5;
                line2.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromArgb(alphaRGB, cr, cg, cb));
                ImageView.Children.Add(line2);
            }
        }

        private void DrawTriangle(List<System.Windows.Point> points)
        {
            double thickness = 0x00;
            double.TryParse(ConfigurationManager.AppSettings["AtomThickness"], out thickness);
            Polygon polygon = new Polygon();
            polygon.StrokeThickness = thickness;
            polygon.Points = new PointCollection(points);
            polygon.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromArgb(alphaRGB, 0, 0, 0));
            polygon.StrokeDashArray = new DoubleCollection((new double[] { 2, 4 }).AsEnumerable());
            polygon.Uid = "Boundary" + Guid.NewGuid().ToString();
            ImageView.Children.Add(polygon);
        }

        private void UpdateDraw()
        {
            if (ViewModeNavi)
                DrawFov(selectedItem);
            else
                DrawMap(selectedItem);
        }

        #endregion Drawing

        #region Others

        public static BitmapImage LoadImageFile(string imgPath)
        {
            BitmapImage retImage = null;

            if (!string.IsNullOrWhiteSpace(imgPath))
            {
                if (System.IO.File.Exists(imgPath))
                {
                    using (FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read, FileShare.Delete))
                    {
                        byte[] data = new byte[fs.Length];
                        fs.ReadAsync(data, 0, (int)fs.Length).Wait();

                        BitmapImage bitmap = new BitmapImage();

                        // Tell the WPF BitmapImage to use this stream
                        bitmap.BeginInit();
                        bitmap.StreamSource = new MemoryStream(data);
                        bitmap.CacheOption = BitmapCacheOption.Default;
                        bitmap.EndInit();
                        return bitmap;
                    }
                }
            }
            return retImage;
        }

        public static System.Windows.Point PointCoorToPointWindow(Coordinate pF)
        {
            return new System.Windows.Point(pF.X, pF.Y);
        }

        public static RectangleF RatioRect(PointF rectCenter, SizeF rectSize, float ratioX, float ratioY)
        {
            float width = rectSize.Width * ratioX;
            float height = rectSize.Height * ratioY;
            float centerX = rectCenter.X * ratioX;
            float centerY = rectCenter.Y * ratioY;

            return new RectangleF(centerX - width / 2, centerY - height / 2, width, height);
        }

        private static void GetColorArray(int arrIdx, out byte cr, out byte cg, out byte cb, JobReader.DrawData.DataType dataType = DrawData.DataType.BODY)
        {
            cr = 0x00;
            cg = 0x00;
            cb = 0x00;
            switch (dataType)
            {
                case DrawData.DataType.BODY:
                    long colorbase = 0xFFFF0000FF;
                    long colorTic = colorbase;
                    int shift = ((arrIdx - 1) * 6) % 24;
                    for (int i = 0; i < shift; i++)
                        colorTic = colorTic >> 1;
                    cr = (byte)(colorTic & 0xFF);
                    cg = (byte)(colorTic >> 8 & 0xFF);
                    cb = (byte)(colorTic >> 16 & 0xFF);
                    if (cr >= 0x80) cr = 0xE0;
                    if (cg >= 0x80) cg = 0xE0;
                    if (cb >= 0x80) cb = 0xE0;
                    break;

                case DrawData.DataType.LEAD:
                    byte.TryParse(ConfigurationManager.AppSettings["crLead"], out cr);
                    byte.TryParse(ConfigurationManager.AppSettings["cgLead"], out cg);
                    byte.TryParse(ConfigurationManager.AppSettings["cbLead"], out cb);
                    break;

                case DrawData.DataType.PAD:
                    byte.TryParse(ConfigurationManager.AppSettings["crPad"], out cr);
                    byte.TryParse(ConfigurationManager.AppSettings["cgPad"], out cg);
                    byte.TryParse(ConfigurationManager.AppSettings["cbPad"], out cb);
                    break;

                default:
                    break;
            }
        }

        private void ChoiceDrawType()
        {
            bool isIts = false;
            clusterConvexPol.ToList().ForEach((p) =>
            {
                isIts = clusterConvexPol.ToList().Where(q => q.Value.Intersects(p.Value) && q.Key != p.Key)?.ToList().Count > 0;
                if (isIts && !isRedraw)
                {
                    rdoCombineIntersect.IsChecked = true;
                    grdTriangulationOption.IsEnabled = false;
                    drawType = DrawType.COMBINE_INTERSECTION;
                    return;
                }
            });
            if (!isIts && !isRedraw)
            {
                rdoConvexHull.IsChecked = true;
                drawType = DrawType.CONVEX;
            }
        }

        private void CombineIntersectionArea(int clusterId, int clusterNextId, Geometry geoIntersect)
        {
            double thresold = 0x00, redemptionNumber = 0x00;
            double.TryParse(ConfigurationManager.AppSettings["RedemptionNumber"], out redemptionNumber);
            double.TryParse(ConfigurationManager.AppSettings["ThresholdArea"], out thresold);
            Geometry unionpoly1 = null, unionpoly2 = null;

            if (geoIntersect.Area / redemptionNumber > thresold)
            {
                //find missing Convex
                var missingPoints1 = boardPoints[clusterId].Where(dic => geoIntersect.Relate(dic.Value).IsCovers() || geoIntersect.Relate(dic.Value).IsContains()).Select(dic => dic.Value).ToArray();
                var missingPoints2 = boardPoints[clusterNextId].Where(dic => geoIntersect.Relate(dic.Value).IsCovers() || geoIntersect.Relate(dic.Value).IsContains()).Select(dic => dic.Value).ToArray();
                var missingEnvelop1 = ConvexHull.Create(missingPoints1);
                var missingEnvelop2 = ConvexHull.Create(missingPoints2);

                //find not missing Convex
                var notMissingPoints1 = boardPoints[clusterId].Where(dic => !geoIntersect.Relate(dic.Value).IsCovers() && !geoIntersect.Relate(dic.Value).IsContains()).Select(dic => dic.Value).ToArray();
                var notMissingPoints2 = boardPoints[clusterNextId].Where(dic => !geoIntersect.Relate(dic.Value).IsCovers() && !geoIntersect.Relate(dic.Value).IsContains()).Select(dic => dic.Value).ToArray();
                var notMissingEnvelop1 = ConvexHull.Create(notMissingPoints1);
                var notMissingEnvelop2 = ConvexHull.Create(notMissingPoints2);

                //find minimum not missing Concave
                var differencePol1 = clusterConvexPol[clusterId].Difference(clusterConvexPol[clusterNextId]).Intersection(notMissingEnvelop1);
                var differencePol2 = clusterConvexPol[clusterNextId].Difference(clusterConvexPol[clusterId]).Intersection(notMissingEnvelop2);

                //Union missing Convex
                unionpoly1 = differencePol1; unionpoly2 = differencePol2;
                if (missingEnvelop1.Intersects(differencePol1))
                    unionpoly1 = missingEnvelop1.Union(differencePol1);
                else
                    unionpoly1 = unionpoly1.Union(ConvexHull.Create(missingEnvelop1.Coordinates.Concat(differencePol1.Centroid.Coordinates).ToList().ConvertAll(p => new Point(p))));

                //Reduce minimum not missing Concave by other missing buffer region
                int bufferSize = 0;
                while (bufferSize < 100 && !boardPoints[clusterId].Any(p => missingEnvelop2.Contains(p.Value) || missingEnvelop2.Covers(p.Value)))
                    bufferSize += 10;
                unionpoly1 = unionpoly1.Difference(missingEnvelop2.Buffer(bufferSize));

                //Union missing Convex
                if (missingEnvelop2.Intersects(differencePol2))
                {
                    unionpoly2 = missingEnvelop2.Union(differencePol2);
                }
                else
                {
                    unionpoly2 = unionpoly2.Union(ConvexHull.Create(missingEnvelop2.Coordinates.Concat(differencePol2.Centroid.Coordinates).ToList().ConvertAll(p => new Point(p))));
                }
                //Reduce minimum not missing Concave by other missing buffer region
                bufferSize = 0;
                while (bufferSize < 100 && !boardPoints[clusterNextId].Any(p => missingEnvelop1.Contains(p.Value) || missingEnvelop1.Covers(p.Value)))
                    bufferSize += 10;
                unionpoly2 = unionpoly2.Difference(missingEnvelop1.Buffer(bufferSize));
            }
            else
            {
                unionpoly1 = clusterConvexPol[clusterId];
                unionpoly2 = clusterConvexPol[clusterNextId];
                while (unionpoly1.Intersects(unionpoly2))
                {
                    unionpoly1 = unionpoly1.Buffer(-2);
                    unionpoly2 = unionpoly2.Buffer(-2);
                }
            }

            byte cr = 0, cg = 0, cb = 0;
            if (unionpoly1 != null)
            {
                GetColorArray(clusterId, out cr, out cg, out cb);
                shapeClusters.Add(new ClusterShape(cr, cg, cb, unionpoly1));
            }
            if (unionpoly2 != null)
            {
                GetColorArray(clusterNextId, out cr, out cg, out cb);
                shapeClusters.Add(new ClusterShape(cr, cg, cb, unionpoly2));
            }
        }

        private void FindClusterIntersect(int clusterId, out int intersectPolIdx, out Geometry geoCurrent, out Geometry geoNext, out Geometry geoIntersect)
        {
            intersectPolIdx = -1;
            geoCurrent = clusterConvexPol[clusterId];
            geoNext = null; geoIntersect = null;
            for (int i = 1; i <= clusterConvexPol.Count; i++)
            {
                if (i != clusterId && geoCurrent.Intersects(clusterConvexPol[i]))
                {
                    intersectPolIdx = i;
                    geoNext = clusterConvexPol[i];
                    geoIntersect = geoCurrent.Intersection(geoNext);
                    break;
                }
            }
        }

        private int InitCompRectList(float ratioX, float ratioY, Dictionary<int, Point> clusterArray, int compId, Component comp)
        {
            float minXCom = 0, minYCom = 0, maxXCom = 0, maxYCom = 0;

            minXCom = comp.DrawDatas.Min(drD => RatioRect(drD.AbsLocation, drD.Size, ratioX, ratioY).X);
            minYCom = comp.DrawDatas.Min(drD => RatioRect(drD.AbsLocation, drD.Size, ratioX, ratioY).Y);
            maxXCom = comp.DrawDatas.Max((drD) =>
            {
                var rect = RatioRect(drD.AbsLocation, drD.Size, ratioX, ratioY);
                return rect.X + rect.Width;
            });
            maxYCom = comp.DrawDatas.Max((drD) =>
            {
                var rect = RatioRect(drD.AbsLocation, drD.Size, ratioX, ratioY);
                return rect.Y + rect.Height;
            });

            clusterArray.Add(compId++, new Point(minXCom, minYCom));
            clusterArray.Add(compId++, new Point(minXCom, maxYCom));
            clusterArray.Add(compId++, new Point(maxXCom, maxYCom));
            clusterArray.Add(compId++, new Point(maxXCom, minYCom));

            compBoundarys.Add(new RectangleF(minXCom, minYCom, maxXCom - minXCom, maxYCom - minYCom));
            return compId;
        }

        private void ReduceByEnvelopIntersection(int clusterId, int clusterNextId, Geometry geoIntersect)
        {
            double thresold = 0x00, redemptionNumber = 0x00;
            double.TryParse(ConfigurationManager.AppSettings["ThresholdArea"], out thresold);
            double.TryParse(ConfigurationManager.AppSettings["RedemptionNumber"], out redemptionNumber);
            Geometry unionpoly1 = null, unionpoly2 = null;

            if (geoIntersect.Area / redemptionNumber > thresold)
            {
                //Get missing Envelop
                var missingPoints1 = boardPoints[clusterId].Where(dic => geoIntersect.Relate(dic.Value).IsCovers() || geoIntersect.Relate(dic.Value).IsContains()).Select(dic => dic.Value).ToArray();
                var missingPoints2 = boardPoints[clusterNextId].Where(dic => geoIntersect.Relate(dic.Value).IsCovers() || geoIntersect.Relate(dic.Value).IsContains()).Select(dic => dic.Value).ToArray();
                var missingEnvelop1 = ConvexHull.Create(missingPoints1).Envelope;
                var missingEnvelop2 = ConvexHull.Create(missingPoints2).Envelope;

                //get current triangulation
                DelaunayTriangulationBuilder triangulationBuilder = new DelaunayTriangulationBuilder();
                triangulationBuilder.SetSites(boardPoints[clusterId].Values.ToList().ConvertAll(p => p.Coordinate));
                var convexHull1 = clusterConvexPol[clusterId];

                var delaunayTriangulation1 = triangulationBuilder.GetTriangles(geometryFactory)
                    .Where(tri => tri.OgcGeometryType == OgcGeometryType.Polygon &&
                        tri.Intersects(geoIntersect));

                var triIntersect = geometryFactory.CreateGeometryCollection(delaunayTriangulation1.Where(tri => tri.Intersects(missingEnvelop2)).ToArray()).Union();
                unionpoly1 = convexHull1.Difference(triIntersect.Envelope.Buffer(-40));

                //get intersect triangulation
                triangulationBuilder = new DelaunayTriangulationBuilder();
                triangulationBuilder.SetSites(boardPoints[clusterNextId].Values.ToList().ConvertAll(p => p.Coordinate));
                var convexHull2 = clusterConvexPol[clusterNextId];

                var delaunayTriangulation2 = triangulationBuilder.GetTriangles(geometryFactory)
                     .Where(tri => tri.OgcGeometryType == OgcGeometryType.Polygon &&
                        tri.Intersects(geoIntersect));
                triIntersect = geometryFactory.CreateGeometryCollection(delaunayTriangulation2.Where(tri => tri.Intersects(missingEnvelop1)).ToArray()).Union();
                unionpoly2 = convexHull2.Difference(triIntersect.Envelope.Buffer(-40));
            }
            else
            {
                unionpoly1 = clusterConvexPol[clusterId];
                unionpoly2 = clusterConvexPol[clusterNextId];
                while (unionpoly1.Intersects(unionpoly2))
                {
                    unionpoly1 = unionpoly1.Buffer(-2);
                    unionpoly2 = unionpoly2.Buffer(-2);
                }
            }

            byte cr = 0, cg = 0, cb = 0;
            if (unionpoly1 != null)
            {
                GetColorArray(clusterId, out cr, out cg, out cb);
                shapeClusters.Add(new ClusterShape(cr, cg, cb, unionpoly1));
            }
            if (unionpoly2 != null)
            {
                GetColorArray(clusterNextId, out cr, out cg, out cb);
                shapeClusters.Add(new ClusterShape(cr, cg, cb, unionpoly2));
            }
        }

        private void ReduceByIntersectionTriangle(int clusterId, int clusterNextId, Geometry geoIntersect)
        {
            double thresold = 0x00, redemptionNumber = 0x00;
            double.TryParse(ConfigurationManager.AppSettings["ThresholdArea"], out thresold);
            double.TryParse(ConfigurationManager.AppSettings["RedemptionNumber"], out redemptionNumber);
            Geometry unionpoly1 = null, unionpoly2 = null;

            if (geoIntersect.Area / redemptionNumber > thresold)
            {
                unionpoly1 = clusterConvexPol[clusterId];
                unionpoly2 = clusterConvexPol[clusterNextId];

                //get missing point when intersection 2 cluster
                var missingPoints1 = boardPoints[clusterId].Where(dic => geoIntersect.Relate(dic.Value).IsCovers() || geoIntersect.Relate(dic.Value).IsContains()).Select(dic => dic.Value).ToArray();
                var missingPoints2 = boardPoints[clusterNextId].Where(dic => geoIntersect.Relate(dic.Value).IsCovers() || geoIntersect.Relate(dic.Value).IsContains()).Select(dic => dic.Value).ToArray();
                var missingEnvelop1 = ConvexHull.Create(missingPoints1);
                var missingEnvelop2 = ConvexHull.Create(missingPoints2);

                //get current triangulation
                DelaunayTriangulationBuilder triangulationBuilder = new DelaunayTriangulationBuilder();
                triangulationBuilder.SetSites(boardPoints[clusterId].Values.ToList().ConvertAll(p => p.Coordinate));
                var delaunayTriangulation1 = triangulationBuilder.GetTriangles(geometryFactory).Where(tri => tri.OgcGeometryType == OgcGeometryType.Polygon &&
                        tri.Intersects(geoIntersect)).OrderByDescending(tri => tri.Length).GetEnumerator();

                //get intersect triangulation
                triangulationBuilder = new DelaunayTriangulationBuilder();
                triangulationBuilder.SetSites(boardPoints[clusterNextId].Values.ToList().ConvertAll(p => p.Coordinate));
                var delaunayTriangulation2 = triangulationBuilder.GetTriangles(geometryFactory).Where(tri => tri.OgcGeometryType == OgcGeometryType.Polygon &&
                        tri.Intersects(geoIntersect)).OrderByDescending(tri => tri.Length).GetEnumerator();

                #region Draw Intersection Triangles

                if (chkDrawTriangulation.IsChecked.Value)
                {
                    delaunayTriangulation1.Reset();
                    while (delaunayTriangulation1.MoveNext())
                    {
                        var lstPoints = delaunayTriangulation1.Current.Coordinates.ToList();
                        lstPoints.Add(lstPoints[0]);
                        DrawTriangle(lstPoints.ConvertAll(PointCoorToPointWindow));
                    }
                    delaunayTriangulation2.Reset();
                    while (delaunayTriangulation2.MoveNext())
                    {
                        var lstPoints = delaunayTriangulation2.Current.Coordinates.ToList();
                        lstPoints.Add(lstPoints[0]);
                        DrawTriangle(lstPoints.ConvertAll(PointCoorToPointWindow));
                    }
                }

                #endregion Draw Intersection Triangles

                //reduce triangles
                double len1 = 0, len2 = 0;
                int itsId = 0; bool errorDiff = false;
                while (unionpoly1.Intersects(unionpoly2) && delaunayTriangulation1.MoveNext() && delaunayTriangulation2.MoveNext())
                {
                    itsId++;
                    //Reduce current cluster
                    if (unionpoly1.Intersects(missingEnvelop2))
                    {
                        //if error difference (Len of difference >= Len of reduction polygon + len offer reduction triangle) -> perform buffer triangle
                        len1 = unionpoly1.Length; len2 = delaunayTriangulation1.Current.Length;
                        var diff = unionpoly1.Difference(delaunayTriangulation1.Current);
                        errorDiff = itsId == 1 && diff.Length >= len1 + len2;
                        unionpoly1 = unionpoly1.Difference(delaunayTriangulation1.Current.Buffer(errorDiff ? 20 : 0));
                    }
                    //Reduce intersection cluster
                    if (unionpoly2.Intersects(missingEnvelop1))
                        unionpoly2 = unionpoly2.Difference(delaunayTriangulation2.Current.Buffer(itsId == 1 && errorDiff ? 20 : 0));
                    errorDiff = false;

                    #region Draw Reduction Triangles

                    if (chkIntersectTriangles.IsChecked.Value)
                    {
                        var lstPointsIntersection = delaunayTriangulation1.Current.Coordinates.ToList();
                        lstPointsIntersection.Add(lstPointsIntersection[0]);
                        DrawTriangle(lstPointsIntersection.ConvertAll(PointCoorToPointWindow));

                        lstPointsIntersection = delaunayTriangulation2.Current.Coordinates.ToList();
                        lstPointsIntersection.Add(lstPointsIntersection[0]);
                        DrawTriangle(lstPointsIntersection.ConvertAll(PointCoorToPointWindow));
                    }

                    #endregion Draw Reduction Triangles
                }
            }
            else
            {
                unionpoly1 = clusterConvexPol[clusterId];
                unionpoly2 = clusterConvexPol[clusterNextId];
                while (unionpoly1.Intersects(unionpoly2))
                {
                    unionpoly1 = unionpoly1.Buffer(-2);
                    unionpoly2 = unionpoly2.Buffer(-2);
                }
            }

            byte cr = 0, cg = 0, cb = 0;
            if (unionpoly1 != null)
            {
                GetColorArray(clusterId, out cr, out cg, out cb);
                shapeClusters.Add(new ClusterShape(cr, cg, cb, unionpoly1));
            }
            if (unionpoly2 != null)
            {
                GetColorArray(clusterNextId, out cr, out cg, out cb);
                shapeClusters.Add(new ClusterShape(cr, cg, cb, unionpoly2));
            }
        }

        private void ResetDrawType()
        {
            grdTriangulationOption.IsEnabled = rdoTriangulation.IsChecked.Value;
            if (rdoBBOX.IsChecked.Value) drawType = DrawType.BOUNDINGBOX;
            if (rdoConvexHull.IsChecked.Value) drawType = DrawType.CONVEX;
            if (rdoTriangulation.IsChecked.Value) drawType = DrawType.TRIANGULATION;
            if (rdoCombineIntersect.IsChecked.Value) drawType = DrawType.COMBINE_INTERSECTION;
            if (rdoEnvelopIntersection.IsChecked.Value) drawType = DrawType.ENVELOP_INTERSECTION;
        }

        #endregion Others
    }
}