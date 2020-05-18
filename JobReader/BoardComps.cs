using System.Drawing;

namespace KY.KYV.JobReader
{
    public class Board
    {
        public SizeF Size; //mm base real size;
        public Component[] Components = null;
        public ImageData WholeImage;
        public ImageData[] FovImages;
        public Board() { }

        public Component Component
        {
            get => default;
            set
            {
            }
        }
    }

    public class Component
    {
        public string CRD = "Unknown";
        public int ArrayIndex = 0;
        public int GroupArrayIndex = 0;
        public PointF Location = new PointF(0.0F, 0.0F);
        public float Rotate = 0.0F;
        public DrawData[] DrawDatas = null;

        public Component() { }

        public DrawData DrawData
        {
            get => default;
            set
            {
            }
        }
    }

    public class DrawData
    {
        public PointF Offset = new PointF(0.0F, 0.0F);
        public SizeF Size = new SizeF(0.0F, 0.0F);
        public float Rotate = 0.0F;

        public PointF AbsLocation = new PointF(0.0F, 0.0F);
        public float AbsRoate = 0.0F;

        public enum DataType { NONE, BODY, LEAD, PAD };

        public DataType Type = DataType.NONE;

        public DrawData() { }
    }

    public class ImageData
    {
        public Image Img;
        public SizeF Size;
        public PointF Location;
        /// <summary>
        /// image file full path
        /// </summary>
        public string ImgFilePath;
        public ImageData() { }
    }
    /// <summary>
    /// 부품 회전 좌표 계산
    /// </summary>
    public class MatrixConverter
    {
        System.Drawing.Drawing2D.Matrix mtxCenter = new System.Drawing.Drawing2D.Matrix();
        System.Drawing.Drawing2D.Matrix mtxRotation = new System.Drawing.Drawing2D.Matrix();
        float orgH = 0f;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="rotateX"></param>
        /// <param name="rotateY"></param>
        /// <param name="angle"></param>
        public MatrixConverter(float offsetX, float offsetY, float rotateX, float rotateY, float angle)
        {
            mtxCenter.Translate(offsetX, offsetY);
            mtxRotation.RotateAt(angle, new System.Drawing.PointF(rotateX, rotateY));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="rotateX"></param>
        /// <param name="rotateY"></param>
        /// <param name="angle"></param>
        public MatrixConverter(float offsetX, float offsetY, float angle)
        {
            mtxCenter.Translate(offsetX, offsetY);
            mtxRotation.RotateAt(angle, new System.Drawing.PointF(offsetX, offsetY));
        }

        public static System.Drawing.PointF ULtoLL(float x, float y, float orgh)
        {
            return new System.Drawing.PointF(x, orgh - y);
        }

        public static System.Drawing.PointF LLtoUL(float x, float y, float orgh)
        {
            return new System.Drawing.PointF(x, orgh - y);
        }

        public System.Drawing.PointF TransformPoint(System.Drawing.PointF point, bool bRotate = true)
        {
            return TransformPoint(point.X, point.Y, bRotate);
        }

        public System.Drawing.PointF TransformPoint(float x, float y, bool bRotate = true)
        {
            System.Drawing.PointF[] ptRet = new System.Drawing.PointF[] { new System.Drawing.PointF(x, y) };
            mtxCenter.TransformPoints(ptRet);
            if (bRotate)
                mtxRotation.TransformPoints(ptRet);

            return ptRet[0];
        }
    }
}
