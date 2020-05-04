using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;

namespace KY.KYV.JobReaderDOM
{
    public class ElementHeadInfo
    {
        public static string Name = "headInfo";
        public ElementBoard board;
        public ElementHeadInfo() { }

        public static ElementHeadInfo Create(XElement element)
        {
            if (element == null)
                return null;

            ElementHeadInfo item = new ElementHeadInfo();
            ElementBoard[] boards =
                 (from Element in element.Elements(ElementBoard.Name)
                  select ElementBoard.Create(Element)
                       ).ToArray();
            if (boards.Length > 0)
                item.board = boards[0];
            return item;
        }
    }

    public class ElementBoard
    {
        public static string Name = @"board";

        public string name = "";
        public string si = "";
        public float rot = 0.0F;
        public int bBkImgRot = 0;
        public float w = 0.0F;
        public float h = 0.0F;
        public float orgH = 0.0F;


        public ElementBoard() { }

        public static ElementBoard Create(XElement element)
        {
            if (element == null)
                return null;

            ElementBoard item = new ElementBoard();

            foreach (XAttribute attribute in element.Attributes())
            {
                switch (attribute.Name.LocalName)
                {
                    case "name": item.name = attribute.Value; break;
                    case "si": item.name = attribute.Value; break;
                    case "w": item.w = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "h": item.h = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "rot": item.rot = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "bBkImgRot": item.bBkImgRot = ReaderUtil.ConvertInt32(attribute.Value, 0); break;
                    case "orgH": item.orgH = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                }
            }
            return item;
        }
    }

    #region footprint
    public class ElementFootprints
    {
        public static string Name = "footprints";
        public Dictionary<string, ElementFootprint> ItemDic = new Dictionary<string, ElementFootprint>();
        public ElementFootprints() { }

        public static ElementFootprints Create(XElement element)
        {
            ElementFootprints item = new ElementFootprints();
            item.ItemDic
                = (from Element in element.Elements(ElementFootprint.Name)
                   select ElementFootprint.Create(Element)
                       ).ToDictionary(x => x.name);
            return item;
        }
    }
    public class ElementFootprint
    {
        public static string Name = @"footprint";

        public string name = "";
        public Guid uid = Guid.Empty;
        public ElementPin[] Items;

        public ElementFootprint() { }

        public static ElementFootprint Create(XElement element)
        {
            if (element == null)
                return null;

            ElementFootprint item = new ElementFootprint();

            foreach (XAttribute attribute in element.Attributes())
            {
                switch (attribute.Name.LocalName)
                {
                    case "name": item.name = attribute.Value; break;
                    case "uid": item.uid = ReaderUtil.ConvertGuid(attribute.Value, Guid.Empty); break;
                }
            }

            item.Items
                = (from Element in element.Elements(ElementPin.Name)
                   select ElementPin.Create(Element)
                       ).ToArray();

            return item;
        }
    }
    public class ElementPin
    {
        public static string Name = @"pin";

        public string name = "";
        public Guid uid = Guid.Empty;
        public Guid shuid = Guid.Empty;
        public float x = 0.0f;
        public float y = 0.0f;
        public float rot = 0.0f;

        public ElementPin() { }

        public static ElementPin Create(XElement element)
        {
            ElementPin item = new ElementPin();

            foreach (XAttribute attribute in element.Attributes())
            {
                switch (attribute.Name.LocalName)
                {
                    case "name": item.name = attribute.Value; break;
                    case "uid": item.uid = ReaderUtil.ConvertGuid(attribute.Value, Guid.Empty); break;
                    case "shuid": item.shuid = ReaderUtil.ConvertGuid(attribute.Value, Guid.Empty); break;
                    case "x": item.x = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "y": item.y = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "rot": item.rot = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                }
            }
            return item;
        }
    }
    #endregion

    #region boardarray
    public class ElementBoardArrays
    {
        public static string Name = "boardarrays";
        public ElementBoardArray[] Items;

        public ElementBoardArrays() { }

        public static ElementBoardArrays Create(XElement element)
        {
            if (element == null)
                return null;

            ElementBoardArrays item = new ElementBoardArrays();
            item.Items
                = (from Element in element.Elements(ElementBoardArray.Name)
                   select ElementBoardArray.Create(Element)
                       ).ToArray();
            return item;
        }
    }
    public class ElementBoardArray
    {
        public static string Name = "boardarray";

        public int num = 0;
        public string name = @"";
        public float x = 0.0f;
        public float y = 0.0f;
        public float orgx = 0.0f;
        public float orgy = 0.0f;
        public float rot = 0.0f;
        public bool exec = false;
        public int group = 0;
        public System.Drawing.RectangleF rect;

        public ElementBoardArray() { }

        public static ElementBoardArray Create(XElement element)
        {
            if (element == null)
                return null;

            ElementBoardArray item = new ElementBoardArray();

            foreach (XAttribute attribute in element.Attributes())
            {
                switch (attribute.Name.LocalName)
                {
                    case "num": item.num = ReaderUtil.ConvertInt32(attribute.Value, 0); break;
                    case "name": item.name = attribute.Value; break;
                    case "x": item.x = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "y": item.y = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "orgx": item.orgx = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "orgy": item.orgy = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "rot": item.rot = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "bExec": item.exec = ReaderUtil.ConvertBoolean(attribute.Value, false); break;
                    case "group": item.group = ReaderUtil.ConvertInt32(attribute.Value, 0); break;
                }
            }
            item.rect = new System.Drawing.RectangleF(item.x + 40, (67.73f - 40 - item.y), 10.0f, 10.0f);
            return item;
        }

    }
    #endregion


    #region boardarray
    public class ElementParts
    {
        public static string Name = "parts";
        public Dictionary<string, ElementPart> ItemDic = new Dictionary<string, ElementPart>();
        public ElementParts() { }

        public static ElementParts Create(XElement element)
        {
            if (element == null)
                return null;

            ElementParts item = new ElementParts();
            item.ItemDic
                = (from Element in element.Elements(ElementPart.Name)
                   select ElementPart.Create(Element)
                       ).ToDictionary(x => x.name);
            return item;
        }
    }
    public class ElementPart
    {
        public static string Name = "part";

        public string name = @"";
        public string AlterPart = @"";
        public Guid uid = Guid.Empty;
        public string footprint;
        public string pkg;
        public string orgPkgName;

        public float xoffset = 0.0f;
        public float yoffset = 0.0f;
        public float OffsetAngForUsrRefAng = 0.0f;

        public ElementPackages.ElementPackage elPackage = null;
        public ElementPart() { }

        public static ElementPart Create(XElement element)
        {
            if (element == null)
                return null;

            ElementPart item = new ElementPart();

            foreach (XAttribute attribute in element.Attributes())
            {
                switch (attribute.Name.LocalName)
                {
                    case "name": item.name = attribute.Value; break;
                    case "AlterPart": item.AlterPart = attribute.Value; break;
                    case "uid": item.uid = ReaderUtil.ConvertGuid(attribute.Value, Guid.Empty); break;
                    case "footprint": item.footprint = attribute.Value; break;
                    case "pkg": item.pkg = attribute.Value; break;
                    case "orgPkgName": item.orgPkgName = attribute.Value; break;
                    case "xoffset": item.xoffset = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "yoffset": item.yoffset = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "OffsetAngForUsrRefAng":
                        item.OffsetAngForUsrRefAng = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                }
            }
            return item;
        }
    }
    #endregion

    public class ElementComponents
    {
        public static string Name = "components";
        public ElementComponent[] component;
        public ElementComponents() { }

        public static ElementComponents Create(XElement element)
        {
            if (element == null)
                return null;

            ElementComponents item = new ElementComponents();
            item.component
                = (from Element in element.Elements(ElementComponent.Name)
                   select ElementComponent.Create(Element)
                       ).ToArray();
            return item;
        }
    }

    public class ElementComponent
    {
        public static string Name = "component";

        public string name;
        public string userName;
        public bool bExec;
        public int group;
        public bool Filter;
        public bool bOriExec;
        public string part;
        public Guid partguid;
        public string OrgPart;
        public string AlterPart;
        public string copyPackage;
        public string si;
        public float x;
        public float y;
        public float rot;
        public float fAngleByCompItSelf;
        public bool bModifiedForBadComp;
        public bool bDontAttachPkg;
        public string inspGroupUid;
        public bool bChkAbsence;
        public float fAngleForInsp;
        public float fAngleCutomerOrg;
        public string footprint;
        public int fovPosType;
        public int OrgfovPosType;
        public float fovX;
        public float fovY;
        public int nCompPkgVerification;
        public int nCompInspVerification;
        public float beforeDivideCenterX;
        public float beforeDivideCenterY;
        public bool bDivided;
        public string beforeDivideCompName;
        public float beforeDivideBodyCenterX;
        public float beforeDivideBodyCenterY;
        public RectangleF rect;

        public ElementPart elPart = null;
        public ElementFootprint elFootprint = null;

        public ElementComponent() { }

        public static ElementComponent Create(XElement element)
        {
            if (element == null)
                return null;

            ElementComponent item = new ElementComponent();

            foreach (XAttribute attribute in element.Attributes())
            {
                switch (attribute.Name.LocalName)
                {
                    case "name": item.name = attribute.Value; break;
                    case "userName": item.userName = attribute.Value; break;
                    case "bExec": item.bExec = ReaderUtil.ConvertBoolean(attribute.Value, false); break;
                    case "group": item.group = ReaderUtil.ConvertInt32(attribute.Value, 0); break;
                    case "Filter": item.Filter = ReaderUtil.ConvertBoolean(attribute.Value, false); break;
                    case "bOriExec": item.bOriExec = ReaderUtil.ConvertBoolean(attribute.Value, false); break;
                    case "part": item.part = attribute.Value; break;
                    case "partguid": item.partguid = ReaderUtil.ConvertGuid(attribute.Value, Guid.Empty); break;
                    case "OrgPart": item.OrgPart = attribute.Value; break;
                    case "AlterPart": item.AlterPart = attribute.Value; break;
                    case "copyPackage": item.copyPackage = attribute.Value; break;
                    case "si": item.si = attribute.Value; break;
                    case "x": item.x = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "y": item.y = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "rot": item.rot = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "fAngleByCompItSelf": item.fAngleByCompItSelf = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "bModifiedForBadComp": item.bModifiedForBadComp = ReaderUtil.ConvertBoolean(attribute.Value, false); break;
                    case "bDontAttachPkg": item.bDontAttachPkg = ReaderUtil.ConvertBoolean(attribute.Value, false); break;
                    case "inspGroupUid": item.inspGroupUid = attribute.Value; break;
                    case "bChkAbsence": item.bChkAbsence = ReaderUtil.ConvertBoolean(attribute.Value, false); break;
                    case "fAngleForInsp": item.fAngleForInsp = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "fAngleCutomerOrg": item.fAngleCutomerOrg = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "footprint": item.footprint = attribute.Value; break;
                    case "fovPosType": item.fovPosType = ReaderUtil.ConvertInt32(attribute.Value, 0); break;
                    case "OrgfovPosType": item.OrgfovPosType = ReaderUtil.ConvertInt32(attribute.Value, 0); break;
                    case "fovX": item.fovX = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "fovY": item.fovY = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "nCompPkgVerification": item.nCompPkgVerification = ReaderUtil.ConvertInt32(attribute.Value, 0); break;
                    case "nCompInspVerification": item.nCompInspVerification = ReaderUtil.ConvertInt32(attribute.Value, 0); break;
                    case "beforeDivideCenterX": item.beforeDivideCenterX = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "beforeDivideCenterY": item.beforeDivideCenterY = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "bDivided": item.bDivided = ReaderUtil.ConvertBoolean(attribute.Value, false); break;
                    case "beforeDivideCompName": item.beforeDivideCompName = attribute.Value; break;
                    case "beforeDivideBodyCenterX": item.beforeDivideBodyCenterX = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "beforeDivideBodyCenterY": item.beforeDivideBodyCenterY = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                }
            }

            item.rect = new RectangleF(
                                item.x + 40 + item.x,
                                (67.73f - 40 - item.y) + item.y,
                                10.0f,
                                10.0f);
            return item;
        }
    }
    /// <summary>
    /// pkg
    /// </summary>
    public class ElementPackages
    {
        public static string Name = "pkgs";
        //public ElementPackage[] Items;
        public Dictionary<string, ElementPackage> ItemDic = new Dictionary<string, ElementPackage>();
        public ElementPackages() { }
        public static ElementPackages Create(XElement element)
        {
            if (element == null)
                return null;

            ElementPackages item = new ElementPackages();

            item.ItemDic
                = (from Element in element.Elements(ElementPackage.Name)
                   select ElementPackage.Create(Element)
                       ).ToDictionary(x => x.name);

            return item;
        }
        public class ElementPackage
        {
            public static string Name = "pkg";
            public ElementPackageBody[] pkgbody;
            public ElementPackagePins[] pkgpins;
            //<pkg 
            public string name;// = "277371" 
            public Guid uid;//="{589266CF-C956-4BE3-86DA-43305E7A3586}" 
            public string type;//="CAPACITOR" 
            public string pkgInspGroup;//="Standards" 
            public string maker;//="" 
            public int pkgApplyLevel;//="2" 
            private int _fOrgOffsetAng;//= -999>;
            public int fOrgOffsetAng
            {
                get { return _fOrgOffsetAng; }
                set
                {
                    if (value == -999)
                        _fOrgOffsetAng = 0;
                    else
                        _fOrgOffsetAng = value;
                }
            }

            public ElementPackage() { }
            public static ElementPackage Create(XElement element)
            {
                if (element == null)
                    return null;

                ElementPackage item = new ElementPackage();
                foreach (XAttribute attribute in element.Attributes())
                {
                    switch (attribute.Name.LocalName)
                    {
                        case "name": item.name = attribute.Value; break;
                        case "uid": item.uid = ReaderUtil.ConvertGuid(attribute.Value, Guid.Empty); break;
                        case "type": item.type = attribute.Value; break;
                        case "pkgInspGroup": item.pkgInspGroup = attribute.Value; break;
                        case "maker": item.maker = attribute.Value; break;
                        case "pkgApplyLevel": item.pkgApplyLevel = ReaderUtil.ConvertInt32(attribute.Value, 0); break;
                        case "fOrgOffsetAng": item.fOrgOffsetAng = ReaderUtil.ConvertInt32(attribute.Value, 0); break;
                    }
                }

                item.pkgbody
                    = (from Element in element.Elements(ElementPackageBody.Name)
                       select ElementPackageBody.Create(Element)
                           ).ToArray();
                item.pkgpins
                    = (from Element in element.Elements(ElementPackagePins.Name)
                       select ElementPackagePins.Create(Element)
                           ).ToArray();
                return item;
            }

            public class ElementPackageBody
            {
                public static string Name = "pkgbody";
                //<pkgbody 
                public Guid shuid;// = "{03400597-2365-47CD-B8EF-DA7BC634B0E3}"
                public float x;//="0.00000"
                public float y;// ="0.00000" '
                public float rot;// ="0.00000"
                public float thickness;// ="0.21000"
                public float gap;// ="0.00000"
                public float wptp;// ="0.00000"
                public float hptp;// ="0.00000"
                public float pitch;// ="0.01000"/>

                public ElementPackageBody() { }
                public static ElementPackageBody Create(XElement element)
                {
                    if (element == null)
                        return null;
                    ElementPackageBody item = new ElementPackageBody();

                    foreach (XAttribute attribute in element.Attributes())
                    {
                        switch (attribute.Name.LocalName)
                        {
                            case "shuid": item.shuid = ReaderUtil.ConvertGuid(attribute.Value, Guid.Empty); break;
                            case "x": item.x = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                            case "y": item.y = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                            case "rot": item.rot = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                            case "thickness": item.thickness = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                            case "gap": item.gap = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                            case "wptp": item.wptp = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                            case "hptp": item.hptp = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                            case "pitch": item.pitch = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                        }
                    }
                    return item;
                }
            }

            public class ElementPackagePins
            {
                public static string Name = "pkgpins";
                public ElementPackagePin[] pkgpin;

                public ElementPackagePins() { }
                public static ElementPackagePins Create(XElement element)
                {
                    if (element == null)
                        return null;

                    ElementPackagePins item = new ElementPackagePins();

                    item.pkgpin
                        = (from Element in element.Elements(ElementPackagePin.Name)
                           select ElementPackagePin.Create(Element)
                               ).ToArray();

                    return item;
                }

                public class ElementPackagePin
                {
                    public static string Name = "pkgpin";
                    //<pkgpin 
                    public string name;// = "00001"
                    public string userName;// =""
                    public Guid uid;//="{073A2EB3-4D79-4947-BA3C-49F9BF146B53}" 
                    public int order;//="1" 
                    public Guid shuid;// ="{407F67A0-1F56-4CE8-8E6A-00E954FB597D}" 
                    public float x;//="-0.18500"
                    public float y;// ="-0.00000"
                    public float rot;// ="0.00000"
                    public float thickness;// ="0.21000" 
                    public string type;//="J"
                    public string attachSide;// ="N"/>

                    public ElementPackagePin() { }
                    public static ElementPackagePin Create(XElement element)
                    {
                        if (element == null)
                            return null;
                        ElementPackagePin item = new ElementPackagePin();

                        foreach (XAttribute attribute in element.Attributes())
                        {
                            switch (attribute.Name.LocalName)
                            {
                                case "name": item.name = attribute.Value; break;
                                case "userName": item.userName = attribute.Value; break;
                                case "uid": item.uid = ReaderUtil.ConvertGuid(attribute.Value, Guid.Empty); break;
                                case "order": item.order = ReaderUtil.ConvertInt32(attribute.Value, 0); break;
                                case "shuid": item.shuid = ReaderUtil.ConvertGuid(attribute.Value, Guid.Empty); break;
                                case "x": item.x = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                                case "y": item.y = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                                case "rot": item.rot = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                                case "thickness": item.thickness = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                                case "type": item.type = attribute.Value; break;
                                case "attachSide": item.attachSide = attribute.Value; break;
                            }
                        }

                        return item;
                    }
                }
            }
        }

    }

    public class ElementPackageBodyShapes
    {
        public static string Name = "pkgBodyShapes";
        //public ElementShape[] shape;
        public Dictionary<Guid, ElementShape> ItemDic = new Dictionary<Guid, ElementShape>();
        public ElementPackageBodyShapes() { }
        public static ElementPackageBodyShapes Create(XElement element)
        {
            if (element == null)
                return null;

            ElementPackageBodyShapes item = new ElementPackageBodyShapes();
            item.ItemDic
                = (from Element in element.Elements(ElementShape.Name)
                   select ElementShape.Create(Element)
                       ).ToDictionary(x => x.uid);
            return item;
        }
    }
    public class ElementPackageLeadShapes
    {
        public static string Name = "pkgLeadShapes";
        //public ElementShape[] shape;
        public Dictionary<Guid, ElementShape> ItemDic = new Dictionary<Guid, ElementShape>();
        public ElementPackageLeadShapes() { }
        public static ElementPackageLeadShapes Create(XElement element)
        {
            if (element == null)
                return null;

            ElementPackageLeadShapes item = new ElementPackageLeadShapes();
            item.ItemDic
                = (from Element in element.Elements(ElementShape.Name)
                   select ElementShape.Create(Element)
                       ).ToDictionary(x => x.uid);
            return item;
        }
    }

    public class ElementFootprintLeadShapes
    {
        public static string Name = "footprtLeadShapes";
        //public ElementShape[] shape;
        public Dictionary<Guid, ElementShape> ItemDic = new Dictionary<Guid, ElementShape>();

        public ElementFootprintLeadShapes() { }
        public static ElementFootprintLeadShapes Create(XElement element)
        {
            if (element == null)
                return null;

            ElementFootprintLeadShapes item = new ElementFootprintLeadShapes();
            item.ItemDic
                = (from Element in element.Elements(ElementShape.Name)
                   select ElementShape.Create(Element)
                       ).ToDictionary(x => x.uid);
            return item;
        }
    }

    public class ElementShape
    {
        public static string Name = "shape";
        public ElementRect[] rc;
        public ElementDrawPath[] drawpath;

        public Guid uid;//="{03400597-2365-47CD-B8EF-DA7BC634B0E3}" 
        public Guid name;//="{03400597-2365-47CD-B8EF-DA7BC634B0E3}"


        public ElementShape() { }
        public static ElementShape Create(XElement element)
        {

            ElementShape item = new ElementShape();

            foreach (XAttribute attribute in element.Attributes())
            {
                switch (attribute.Name.LocalName)
                {
                    case "uid": item.uid = ReaderUtil.ConvertGuid(attribute.Value, Guid.Empty); break;
                    case "name": item.name = ReaderUtil.ConvertGuid(attribute.Value, Guid.Empty); break;
                }
            }

            item.rc
                = (from Element in element.Elements(ElementRect.Name)
                   select ElementRect.Create(Element)
                       ).ToArray();
            item.drawpath
                = (from Element in element.Elements(ElementDrawPath.Name)
                   select ElementDrawPath.Create(Element)
                       ).ToArray();

            return item;
        }


    }



    public class ElementDrawPath
    {
        public static string Name = "drawpath";
        ElementPoint[] pt;
        float area;

        public ElementDrawPath() { }
        public static ElementDrawPath Create(XElement element)
        {
            if (element == null)
                return null;

            ElementDrawPath item = new ElementDrawPath();

            foreach (XAttribute attribute in element.Attributes())
            {
                switch (attribute.Name.LocalName)
                {
                    case "area": item.area = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                }
            }

            item.pt
                = (from Element in element.Elements(ElementPoint.Name)
                   select ElementPoint.Create(Element)
                       ).ToArray();
            return item;
        }
    }


    public class ElementRect
    {
        public static string Name = "rc";
        public float width;// = "0.20000"
        public float length;// ="0.40000"
        public float w;// = "0.40000"
        public float h;// = "0.20000"
        public float rot;// = "0.00000"
        public float area;// = "0.000000"
        public ElementRect() { }
        public static ElementRect Create(XElement element)
        {
            if (element == null)
                return null;

            ElementRect item = new ElementRect();

            foreach (XAttribute attribute in element.Attributes())
            {
                switch (attribute.Name.LocalName)
                {
                    case "width": item.width = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "length": item.length = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "w": item.w = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "h": item.h = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "rot": item.rot = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "area": item.area = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                }
            }

            return item;
        }
    }
    public class ElementPoint
    {
        public static string Name = "pt";
        float x;
        float y;
        public ElementPoint() { }
        public static ElementPoint Create(XElement element)
        {
            if (element == null)
                return null;

            ElementPoint item = new ElementPoint();

            foreach (XAttribute attribute in element.Attributes())
            {
                switch (attribute.Name.LocalName)
                {
                    case "x": item.x = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "y": item.y = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                }
            }
            return item;
        }
    }
    /// <summary>
    /// tag boardfovs
    /// </summary>
    public class ElementBoardFovs
    {
        public static string Name = "boardfovs";

        public float width;
        public float height;
        public ElementFov[] fovs;

        public ElementBoardFovs() { }
        public static ElementBoardFovs Create(XElement element)
        {
            if (element == null)
                return null;

            ElementBoardFovs item = new ElementBoardFovs();

            foreach (XAttribute attribute in element.Attributes())
            {
                switch (attribute.Name.LocalName)
                {
                    case "width": item.width = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "height": item.height = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                }
            }

            item.fovs
                = (from Element in element.Elements(ElementFov.Name)
                   select ElementFov.Create(Element)
                       ).ToArray();
            return item;
        }
    }
    /// <summary>
    /// tag fovs
    /// </summary>
    public class ElementFovs
    {
        public static string Name = "fovs";
        public ElementFov[] fovs;

        public ElementFovs() { }
        public static ElementFovs Create(XElement element)
        {
            if (element == null)
                return null;

            ElementFovs item = new ElementFovs();
            item.fovs

                = (from Element in element.Elements(ElementFov.Name)
                   select ElementFov.Create(Element)
                       ).ToArray();
            return item;
        }
    }

    /// <summary>
    /// tag: bkfovs
    /// </summary>
    public class ElementBKFovs
    {
        public static string Name = "bkfovs";
        public ElementFov[] fovs;

        public ElementBKFovs() { }
        public static ElementBKFovs Create(XElement element)
        {
            if (element == null)
                return null;

            ElementBKFovs item = new ElementBKFovs();
            item.fovs

                = (from Element in element.Elements(ElementFov.Name)
                   select ElementFov.Create(Element)
                       ).ToArray();
            return item;
        }
    }
    public class ElementFov
    {
        public static string Name = "fov";
        public int id;// = "0.20000"
        public float left;// ="0.40000"
        public float height;// = "0.40000"
        public float x;
        public float y;

        public ElementFov() { }
        public static ElementFov Create(XElement element)
        {
            if (element == null)
                return null;

            ElementFov item = new ElementFov();

            foreach (XAttribute attribute in element.Attributes())
            {
                switch (attribute.Name.LocalName)
                {
                    case "id": item.id = ReaderUtil.ConvertInt32(attribute.Value, 0); break;
                    case "left": item.left = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "height": item.height = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "x": item.height = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                    case "y": item.height = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
                }
            }

            return item;
        }
    }

    public class ReaderUtil
    {

        public static int ConvertInt32(string Value, int Default)
        {
            int nValue;
            if (Int32.TryParse(Value, out nValue))
            {
                return nValue;
            }
            return Default;
        }
        public static float ConvertSingle(string Value, float Default)
        {
            Single nValue;
            if (Single.TryParse(Value, out nValue))
            {
                return nValue;
            }
            return Default;
        }
        public static bool ConvertBoolean(string Value, bool Default)
        {
            Boolean nValue;
            if (Boolean.TryParse(Value, out nValue))
            {
                return nValue;
            }
            return Default;
        }

        public static Guid ConvertGuid(string Value, Guid Default)
        {
            Guid nValue;
            if (Guid.TryParse(Value, out nValue))
            {
                return nValue;
            }
            return Default;
        }

        public static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }
        public static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }
    }
}
