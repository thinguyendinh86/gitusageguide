#define JobReaderSAX
//#define JobReaderDOM

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

#if JobReaderSAX
using KY.KYV.JobReader.JobReaderSAX;
#elif JobReaderDOM
using KY.KYV.JobReader.JobReaderDOM;
#endif



namespace KY.KYV.JobReader
{
    public class Reader : IDisposable
    {
        /// <summary>
        /// Array 정보
        /// </summary>
        public ElementBoardArrays BoardArrays;
        /// <summary>
        /// 패드 정보
        /// </summary>
        public ElementFootprints Footprints;
        /// <summary>
        /// 파트 정보
        /// </summary>
        public ElementParts Parts;
        /// <summary>
        /// 패키지 정보
        /// </summary>
        public ElementPackages Packages;
        /// <summary>
        /// 부품 정보 
        /// </summary>
        public ElementComponents Components;
        /// <summary>
        /// 형상정보
        /// </summary>
        public ElementPackageBodyShapes PackageBodyShapes;
        public ElementPackageLeadShapes PackageLeadShapes;
        public ElementFootprintLeadShapes FootprintLeadShapes;


        /// <summary>
        /// 검사영역 정보
        /// </summary>
        public ElementBoardFovs Boardfovs;
        public ElementFovs Fovs;
        public ElementBKFovs BkFovs;

        /// <summary>
        /// PCB 기본정보
        /// </summary>
        public ElementHeadInfo HeadInfo;


        /// <summary>
        /// PCB정보를 화면에 그리기 위한 정보
        /// </summary>
        public Board BoardData;


        public string JobfilePath = "";
        public string JobData = "";
        /// <summary>
        /// Job파일 XML 구조 읽기용.
        /// </summary>
        public Reader()
        {

        }

        /// <summary>
        /// Job 파일 열기
        /// </summary>
        /// <param name="filePath">Job파일 절대 경로</param>
        /// <returns></returns>
        public bool OpenFile(string filePath)
        {
            bool bRet = false;
            using (Stream stream = GetResourceStream(filePath))
            {
                using (XmlReader xmlRdr = new XmlTextReader(stream))
                {
                    bRet = AnalyzeData(xmlRdr);
                }
            }
            if (bRet)
                JobfilePath = filePath;
            return bRet;
        }

        /// <summary>
        /// Job 파일 열기
        /// </summary>
        /// <param name="xmlData">Job파일 내용</param>
        /// <returns></returns>
        public bool OpenData(string xmlData)
        {
            bool bRet = false;
            using (XmlReader xmlRdr = XmlReader.Create(new StringReader(xmlData)))
            {

                bRet = AnalyzeData(xmlRdr);
            }

            if (bRet)
            {
                JobData = xmlData;
            }

            return bRet;
        }
#if JobReaderSAX
        private bool AnalyzeData(XmlReader xmlRdr)
        {
            xmlRdr.MoveToContent();
            int readinfoChecker = 0;
            int checkend = 0xFFF;

            while (xmlRdr.Read())
            {
                if (xmlRdr.NodeType == XmlNodeType.Element)
                {
                    if (string.Compare(xmlRdr.Name, ElementHeadInfo.Name, true) == 0)
                    {
                        HeadInfo = ElementHeadInfo.Create(xmlRdr);
                        readinfoChecker |= 0x1;
                    }
                    else if (string.Compare(xmlRdr.Name, ElementParts.Name, true) == 0)
                    {
                        Parts = ElementParts.Create(xmlRdr);
                        readinfoChecker |= 0x2;
                    }
                    else if (string.Compare(xmlRdr.Name, ElementPackages.Name, true) == 0)
                    {
                        Packages = ElementPackages.Create(xmlRdr);
                        readinfoChecker |= 0x4;
                    }
                    else if (string.Compare(xmlRdr.Name, ElementComponents.Name, true) == 0)
                    {
                        Components = ElementComponents.Create(xmlRdr);
                        readinfoChecker |= 0x8;
                    }
                    else if (string.Compare(xmlRdr.Name, ElementBoardArrays.Name, true) == 0)
                    {
                        BoardArrays = ElementBoardArrays.Create(xmlRdr);
                        readinfoChecker |= 0x10;
                    }


                    else if (string.Compare(xmlRdr.Name, ElementFootprints.Name, true) == 0)
                    {
                        Footprints = ElementFootprints.Create(xmlRdr);
                        readinfoChecker |= 0x20;
                    }
                    else if (string.Compare(xmlRdr.Name, ElementPackageBodyShapes.Name, true) == 0)
                    {
                        PackageBodyShapes = ElementPackageBodyShapes.Create(xmlRdr);
                        readinfoChecker |= 0x40;
                    }
                    else if (string.Compare(xmlRdr.Name, ElementPackageLeadShapes.Name, true) == 0)
                    {
                        PackageLeadShapes = ElementPackageLeadShapes.Create(xmlRdr);
                        readinfoChecker |= 0x80;
                    }

                    else if (string.Compare(xmlRdr.Name, ElementFootprintLeadShapes.Name, true) == 0)
                    {
                        FootprintLeadShapes = ElementFootprintLeadShapes.Create(xmlRdr);
                        readinfoChecker |= 0x100;
                    }
                    else if (string.Compare(xmlRdr.Name, ElementBoardFovs.Name, true) == 0)
                    {
                        Boardfovs = ElementBoardFovs.Create(xmlRdr);
                        readinfoChecker |= 0x200;
                    }
                    else if (string.Compare(xmlRdr.Name, ElementFovs.Name, true) == 0)
                    {
                        Fovs = ElementFovs.Create(xmlRdr);
                        readinfoChecker |= 0x400;
                    }
                    else if (string.Compare(xmlRdr.Name, ElementBKFovs.Name, true) == 0)
                    {
                        BkFovs = ElementBKFovs.Create(xmlRdr);
                        readinfoChecker |= 0x800;
                    }


                    else if (!xmlRdr.IsEmptyElement)
                    {
                        xmlRdr.Skip();
                    }

                    if ((readinfoChecker & checkend) == checkend)
                        break;
                }
            }

            return true;
        }
#elif JobReaderDOM
        private bool AnalyzeData(XmlReader xmlRdr)
        {
            XElement rootElement = XDocument.Load(xmlRdr).Element("KYJOB");


            HeadInfo = ElementHeadInfo.Create(rootElement.Element(ElementHeadInfo.Name));
            BoardArrays = ElementBoardArrays.Create(rootElement.Element(ElementBoardArrays.Name));
            Footprints = ElementFootprints.Create(rootElement.Element(ElementFootprints.Name));
            Parts = ElementParts.Create(rootElement.Element(ElementParts.Name));
            Packages = ElementPackages.Create(rootElement.Element(ElementPackages.Name));
            PackageBodyShapes = ElementPackageBodyShapes.Create(rootElement.Element(ElementPackageBodyShapes.Name));
            PackageLeadShapes = ElementPackageLeadShapes.Create(rootElement.Element(ElementPackageLeadShapes.Name));
            FootprintLeadShapes = ElementFootprintLeadShapes.Create(rootElement.Element(ElementFootprintLeadShapes.Name));
            Components = ElementComponents.Create(rootElement.Element(ElementComponents.Name));
            Boardfovs = ElementBoardFovs.Create(rootElement.Element(ElementBoardFovs.Name));
            Fovs = ElementFovs.Create(rootElement.Element(ElementFovs.Name));
            BkFovs = ElementBKFovs.Create(rootElement.Element(ElementBKFovs.Name));

            return true;
        }
#endif
        private List<string> ReadElementValueList(XElement element, string ElementName)
        {
            return (from Element in element.Elements(ElementName)
                    select Element.Value).ToList();
        }

        static Stream GetResourceStream(string resourceFile)
        {
            return File.Open(resourceFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
        /// <summary>
        /// Reader의 내용을 이용하여 부품의 좌표계를 변환
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static Board ConvertBoardData(Reader reader)
        {
            List<KY.KYV.JobReader.Component> compList = new List<KY.KYV.JobReader.Component>();
            if (reader != null)
            {
                #region ComponentInfo
                if (reader.BoardArrays == null)
                    return null;
                foreach (ElementBoardArray arr in reader.BoardArrays.Items)
                {
                    MatrixConverter arrMtx = new MatrixConverter(arr.x, arr.y, arr.rot);

                    foreach (ElementComponent comp in reader.Components.component)
                    {
                        //get part of comp
                        ElementPart part = reader.Parts.ItemDic[comp.part];

                        //get pack of part
                        ElementPackages.ElementPackage Package;
                        if (!reader.Packages.ItemDic.TryGetValue(part.pkg, out Package))
                            continue;

                        //get rotate of comp
                        float rotate = comp.fAngleByCompItSelf + comp.fAngleForInsp + part.OffsetAngForUsrRefAng;// + Package.fOrgOffsetAng;                                                
                        MatrixConverter compMtx = new MatrixConverter(comp.x, reader.HeadInfo.board.orgH - comp.y, rotate);

                        List<KY.KYV.JobReader.DrawData> drawDataList = new List<KY.KYV.JobReader.DrawData>();

                        //init data for draw body of comp
                        float angleSum = arr.rot + rotate;
                        float angleAdjust = 0.0F;
                        if (Package != null && Package.pkgbody != null)
                            foreach (ElementPackages.ElementPackage.ElementPackageBody pkgbody in Package.pkgbody)
                            {
                                ElementShape shape = reader.PackageBodyShapes.ItemDic[pkgbody.shuid];
                                foreach (ElementRect rect in shape.rc)
                                {
                                    float rectrot = (pkgbody.rot + rect.rot);
                                    MatrixConverter bodyMtx = new MatrixConverter(pkgbody.x, pkgbody.y, rectrot);

                                    PointF ptRect = bodyMtx.TransformPoint(0, 0);
                                    ptRect = compMtx.TransformPoint(ptRect);
                                    ptRect = arrMtx.TransformPoint(ptRect);

                                    drawDataList.Add(
                                        new DrawData()
                                        {
                                            Offset = new System.Drawing.PointF(pkgbody.x, pkgbody.y),
                                            Rotate = (pkgbody.rot + rect.rot),
                                            Size = new System.Drawing.SizeF(rect.w, rect.h),
                                            Type = DrawData.DataType.BODY,
                                            AbsLocation = new System.Drawing.PointF(ptRect.X, reader.HeadInfo.board.orgH - ptRect.Y),
                                            AbsRoate = (pkgbody.rot + rect.rot + angleSum)
                                        });
                                }
                            }

                        //init data for draw footprint of comp
                        if (Package != null && Package.pkgpins != null)
                            foreach (ElementPackages.ElementPackage.ElementPackagePins pkgpins in Package.pkgpins)
                            {
                                foreach (ElementPackages.ElementPackage.ElementPackagePins.ElementPackagePin pkgpin in pkgpins.pkgpin)
                                {
                                    ElementShape shape = reader.PackageLeadShapes.ItemDic[pkgpin.shuid];
                                    foreach (ElementRect rect in shape.rc)
                                    {
                                        float rectrot = (pkgpin.rot + rect.rot);
                                        MatrixConverter pinMtx = new MatrixConverter(pkgpin.x, pkgpin.y, rectrot);

                                        PointF ptRect = pinMtx.TransformPoint(0, 0);
                                        ptRect = compMtx.TransformPoint(ptRect);
                                        ptRect = arrMtx.TransformPoint(ptRect);

                                        drawDataList.Add(
                                            new DrawData()
                                            {
                                                Offset = new System.Drawing.PointF(pkgpin.x, pkgpin.y),
                                                Rotate = (pkgpin.rot + rect.rot),
                                                Size = new System.Drawing.SizeF(rect.w, rect.h),
                                                Type = DrawData.DataType.LEAD,
                                                AbsLocation = new System.Drawing.PointF(ptRect.X, reader.HeadInfo.board.orgH - ptRect.Y),
                                                AbsRoate = (pkgpin.rot + rect.rot + angleSum)

                                            });
                                    }
                                }
                            }


                        //if (reader.Footprints.ItemDic.ContainsKey(comp.footprint))
                        //{
                        //    ElementFootprint footprint = reader.Footprints.ItemDic[comp.footprint];
                        //    foreach (ElementPin pin in footprint.Items)
                        //    {


                        //        if (reader.FootprintLeadShapes.ItemDic.ContainsKey(pin.shuid))
                        //        {
                        //            ElementShape shape = reader.FootprintLeadShapes.ItemDic[pin.shuid];
                        //            foreach (ElementRect rect in shape.rc)
                        //            {
                        //                System.Drawing.Drawing2D.Matrix mtxRectCenter = new System.Drawing.Drawing2D.Matrix();
                        //                mtxRectCenter.Translate(pin.x, pin.y);
                        //                System.Drawing.Drawing2D.Matrix mtxRectRot = new System.Drawing.Drawing2D.Matrix();
                        //                mtxRectRot.RotateAt(-(pin.rot + rect.rot), new PointF(pin.x, pin.y));

                        //                PointF[] ptRect = new PointF[] { new PointF(0F, 0F) };
                        //                mtxRectCenter.TransformPoints(ptRect);
                        //                mtxRectRot.TransformPoints(ptRect);
                        //                mtxCompCenter.TransformPoints(ptRect);
                        //                mtxCompRot.TransformPoints(ptRect);
                        //                mtxArrayCenter.TransformPoints(ptRect);
                        //                mtxArrayRot.TransformPoints(ptRect);

                        //                drawDataList.Add(
                        //                    new DrawData()
                        //                    {
                        //                        Offset = new System.Drawing.PointF(pin.x, pin.y),
                        //                        Rotate = (pin.rot + rect.rot),
                        //                        Size = new System.Drawing.SizeF(rect.w, rect.h),
                        //                        Type = DrawData.DataType.PAD,
                        //                        AbsLocation = new System.Drawing.PointF(ptRect[0].X, ptRect[0].Y),
                        //                        AbsRoate = (pin.rot + rect.rot + angleSum)
                        //                    });
                        //            }

                        //        }
                        //    }
                        //}


                        //init component list data
                        PointF ptComp = compMtx.TransformPoint(0, 0);
                        ptComp = arrMtx.TransformPoint(ptComp);

                        compList.Add(
                            new KY.KYV.JobReader.Component()
                            {
                                CRD = comp.name,
                                ArrayIndex = arr.num,
                                Location = new System.Drawing.PointF(ptComp.X, reader.HeadInfo.board.orgH - ptComp.Y),
                                Rotate = (comp.fAngleByCompItSelf + comp.fAngleForInsp + angleAdjust + Package.fOrgOffsetAng),
                                DrawDatas = drawDataList.ToArray()
                            });

                        System.Diagnostics.Debug.WriteLine($"{comp.name}/ {arr.num}");
                    }
                }

                #endregion

                #region PCBImage

                string imagefilepath = System.IO.Path.GetDirectoryName(reader.JobfilePath);

                string[] ImagefileNameList = new string[] { "WholeboardFov.jpg", "WholeBoard.jpg", "WholeBoard.bmp" };
                Image boardIamge = null;
                string boardimagepath = null;
                foreach (string fileName in ImagefileNameList)
                {
                    System.Diagnostics.Debug.WriteLine(System.IO.Path.Combine(imagefilepath, fileName));
                    if (System.IO.File.Exists(System.IO.Path.Combine(imagefilepath, fileName)))
                    {
                        boardimagepath = System.IO.Path.Combine(imagefilepath, fileName);
                        // boardIamge = Image.FromFile(boardimagepath);
                        break;
                    }
                }

                List<ImageData> fovimages = new List<ImageData>();
                foreach (ElementFov fov in reader.Boardfovs.fovs)
                {
                    Image fovImage = null;
                    string fovImagepath = null;
                    string path = System.IO.Path.Combine(imagefilepath, string.Format("Board-Fov-{0:D3}_T.jpg", fov.id + 1));
                    if (System.IO.File.Exists(path))
                    {
                        fovImagepath = path;
                    }

                    fovimages.Add(new ImageData()
                    {
                        Img = fovImage,
                        ImgFilePath = fovImagepath,
                        Size = new System.Drawing.SizeF(reader.Boardfovs.width, reader.Boardfovs.height),
                        Location = new PointF(fov.left, fov.height)
                    });
                }

                #endregion

                Board board = new Board()
                {
                    Size = new System.Drawing.SizeF(reader.HeadInfo.board.w, reader.HeadInfo.board.h),
                    WholeImage = new ImageData()
                    {
                        Img = boardIamge,
                        ImgFilePath = boardimagepath,
                        Size = new System.Drawing.SizeF(reader.HeadInfo.board.w, reader.HeadInfo.board.h),
                        Location = new PointF(reader.HeadInfo.board.w / 2, reader.HeadInfo.board.h / 2)
                    },
                    FovImages = fovimages.ToArray(),
                    Components = compList.ToArray()
                };
                return board;
            }
            return null;
        }

        #region IDisposable Support
        private bool disposedValue = false; // 중복 호출을 검색하려면

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 관리되는 상태(관리되는 개체)를 삭제합니다.
                }

                // TODO: 관리되지 않는 리소스(관리되지 않는 개체)를 해제하고 아래의 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.

                disposedValue = true;
            }
        }

        // TODO: 위의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다.
        // ~Reader() {
        //   // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
        //   Dispose(false);
        // }

        // 삭제 가능한 패턴을 올바르게 구현하기 위해 추가된 코드입니다.
        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
            Dispose(true);
            // TODO: 위의 종료자가 재정의된 경우 다음 코드 줄의 주석 처리를 제거합니다.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
