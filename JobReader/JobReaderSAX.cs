using System;
using System.Collections.Generic;
using System.Xml;

namespace KY.KYV.JobReader.JobReaderSAX
{
    #region pcbinfo
    public class ElementHeadInfo
    {
        public const string Name = "headInfo";
        public ElementCondVendor condVendor;
        public ElementCriticalDistances criticalDistances;
        public ElementBoard board;
        public string name = "";
        public ElementHeadInfo() { }

        public ElementCondVendor ElementCondVendor
        {
            get => default;
            set
            {
            }
        }

        public ElementBoard ElementBoard
        {
            get => default;
            set
            {
            }
        }

        public ElementCriticalDistances ElementCriticalDistances
        {
            get => default;
            set
            {
            }
        }

        public static ElementHeadInfo Create(XmlReader element)
        {
            if (element == null)
                return null;

            ElementHeadInfo item = new ElementHeadInfo();

            if (!element.IsEmptyElement)
            {
                while (element.Read())
                {
                    if (element.NodeType == XmlNodeType.Whitespace)
                        continue;
                    if (element.NodeType == XmlNodeType.Element && string.Compare(element.Name, ElementCondVendor.Name, true) == 0)
                    {
                        item.condVendor = ElementCondVendor.Create(element);
                    }
                    else if (element.NodeType == XmlNodeType.Element && string.Compare(element.Name, ElementCriticalDistances.Name, true) == 0)
                    {
                        item.criticalDistances = ElementCriticalDistances.Create(element);
                    }
                    else if (element.NodeType == XmlNodeType.Element && string.Compare(element.Name, ElementBoard.Name, true) == 0)
                    {
                        item.board = ElementBoard.Create(element);
                    }
                    else if (element.NodeType == XmlNodeType.EndElement && string.Compare(element.Name, ElementHeadInfo.Name, true) == 0)
                        break;
                    else if (!element.IsEmptyElement)
                    {
                        element.Skip();
                    }
                }
            }
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

        public static ElementBoard Create(XmlReader element)
        {
            if (element == null)
                return null;

            ElementBoard item = new ElementBoard();

            if (element.NodeType == XmlNodeType.Element && element.HasAttributes)
            {
                while (element.MoveToNextAttribute())
                    switch (element.Name)
                    {
                        case "name": item.name = element.Value; break;
                        case "si": item.name = element.Value; break;
                        case "w": item.w = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "h": item.h = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "rot": item.rot = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "bBkImgRot": item.bBkImgRot = ReaderUtil.ConvertInt32(element.Value, 0); break;
                        case "orgH": item.orgH = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                    }

            }
            return item;

        }
    }

    public class ElementCondVendor
    {
        public const string Name = "condVendor";
        public string name = "";
        public ElementCondVendor() { }

        public static ElementCondVendor Create(XmlReader element)
        {
            if (element == null)
                return null;

            ElementCondVendor item = new ElementCondVendor();

            if (element.NodeType == XmlNodeType.Element && element.HasAttributes)
            {
                while (element.MoveToNextAttribute())
                    switch (element.Name)
                    {
                        case "name": item.name = element.Value; break;
                    }
            }
            return item;
        }
    }

    public class ElementCriticalDistances
    {
        public const string Name = "CriticalDistance";
        public ElementCriticalDistance[] Items;

        public ElementCriticalDistances() { }

        public ElementCriticalDistance ElementCriticalDistance
        {
            get => default;
            set
            {
            }
        }

        public static ElementCriticalDistances Create(XmlReader element)
        {
            if (element == null)
                return null;

            ElementCriticalDistances item = new ElementCriticalDistances();


            List<ElementCriticalDistance> newItems = new List<ElementCriticalDistance>();
            if (!element.IsEmptyElement)
                while (element.Read())
                {
                    if (element.NodeType == XmlNodeType.Whitespace)
                        continue;
                    if (element.NodeType == XmlNodeType.Element && string.Compare(element.Name, ElementCriticalDistance.Name, true) == 0)
                    {
                        ElementCriticalDistance newElement = ElementCriticalDistance.Create(element);
                        newItems.Add(newElement);

                    }
                    else if (element.NodeType == XmlNodeType.EndElement && string.Compare(element.Name, ElementCriticalDistances.Name, true) == 0)
                        break;
                }
            item.Items = newItems.ToArray();

            return item;
        }

    }

    public class ElementCriticalDistance
    {
        public const string Name = "CD";
        #region property
        public string name = @"";
        public string displayName = @"";
        public int inspType = 0;
        public int refAxis = 0;
        // 2017-12-27 yj.lee [#53497] circleCenterX,Y 변수 추가 및 초기화
        public float circleCenterX = 0;
        public float circleCenterY = 0;
        public string group = "";
        #endregion
        public ElementCriticalDistance() { }

        public static ElementCriticalDistance Create(XmlReader element)
        {
            ElementCriticalDistance item = new ElementCriticalDistance();

            if (element.NodeType == XmlNodeType.Element && element.HasAttributes)
            {
                while (element.MoveToNextAttribute())
                    switch (element.Name)
                    {
                        case "Group": item.group = element.Value; break;
                        case "name": item.name = element.Value; break;
                        case "DisplayName": item.displayName = element.Value; break;
                        case "InspType": item.inspType = ReaderUtil.ConvertInt32(element.Value, 0); break;
                        case "RefAxis": item.refAxis = ReaderUtil.ConvertInt32(element.Value, 0); break;
                        case "CircleCenterX": item.circleCenterX = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "CircleCenterY": item.circleCenterX = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                    }
            }
            return item;
        }
    }


    #endregion

    #region footprint
    public class ElementFootprints
    {
        public const string Name = "footprints";
        public Dictionary<string, ElementFootprint> ItemDic = new Dictionary<string, ElementFootprint>();
        public ElementFootprints() { }

        public ElementFootprint ElementFootprint
        {
            get => default;
            set
            {
            }
        }

        public static ElementFootprints Create(XmlReader element)
        {
            ElementFootprints item = new ElementFootprints();
            if (!element.IsEmptyElement)
                while (element.Read())
                {
                    if (element.NodeType == XmlNodeType.Whitespace)
                        continue;
                    if (element.NodeType == XmlNodeType.Element && string.Compare(element.Name, ElementFootprint.Name, true) == 0)
                    {
                        ElementFootprint newElement = ElementFootprint.Create(element);
                        item.ItemDic.Add(newElement.name, newElement);

                    }
                    else if (element.NodeType == XmlNodeType.EndElement && string.Compare(element.Name, ElementFootprints.Name, true) == 0)
                        break;
                }
            //item.ItemDic.
            //    = (from Element in element.Elements(ElementFootprint.Name)
            //       select ElementFootprint.Create(Element)
            //           ).ToDictionary(x => x.name);
            return item;
        }
    }
    public class ElementFootprint
    {
        public const string Name = @"footprint";

        public string name = "";
        public Guid uid = Guid.Empty;
        public ElementPin[] Items;

        public ElementFootprint() { }

        public ElementPin ElementPin
        {
            get => default;
            set
            {
            }
        }

        public static ElementFootprint Create(XmlReader element)
        {
            if (element == null)
                return null;

            ElementFootprint item = new ElementFootprint();


            if (element.NodeType == XmlNodeType.Element && element.HasAttributes)
            {
                while (element.MoveToNextAttribute())
                    switch (element.Name)
                    {
                        case "name": item.name = element.Value; break;
                        case "uid": item.uid = ReaderUtil.ConvertGuid(element.Value, Guid.Empty); break;
                    }
            }

            List<ElementPin> newItems = new List<ElementPin>();
            if (!element.IsEmptyElement)
                while (element.Read())
                {
                    if (element.NodeType == XmlNodeType.Whitespace)
                        continue;
                    if (element.NodeType == XmlNodeType.Element && string.Compare(element.Name, ElementPin.Name, true) == 0)
                    {
                        ElementPin newElement = ElementPin.Create(element);
                        newItems.Add(newElement);

                    }
                    else if (element.NodeType == XmlNodeType.EndElement && string.Compare(element.Name, ElementFootprint.Name, true) == 0)
                        break;
                }
            item.Items = newItems.ToArray();

            return item;
        }
    }
    public class ElementPin
    {
        public const string Name = @"pin";

        public string name = "";
        public Guid uid = Guid.Empty;
        public Guid shuid = Guid.Empty;
        public float x = 0.0f;
        public float y = 0.0f;
        public float rot = 0.0f;

        public ElementPin() { }

        public ElementShape ElementShape
        {
            get => default;
            set
            {
            }
        }

        public static ElementPin Create(XmlReader element)
        {
            ElementPin item = new ElementPin();

            if (element.NodeType == XmlNodeType.Element && element.HasAttributes)
            {
                while (element.MoveToNextAttribute())
                    switch (element.Name)
                    {
                        case "name": item.name = element.Value; break;
                        case "uid": item.uid = ReaderUtil.ConvertGuid(element.Value, Guid.Empty); break;
                        case "shuid": item.shuid = ReaderUtil.ConvertGuid(element.Value, Guid.Empty); break;
                        case "x": item.x = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "y": item.y = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "rot": item.rot = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                    }
            }


            //foreach (XAttribute attribute in element.Attributes())
            //{
            //    switch (attribute.Name.LocalName)
            //    {
            //        case "name": item.name = attribute.Value; break;
            //        case "uid": item.uid = ReaderUtil.ConvertGuid(attribute.Value, Guid.Empty); break;
            //        case "shuid": item.shuid = ReaderUtil.ConvertGuid(attribute.Value, Guid.Empty); break;
            //        case "x": item.x = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
            //        case "y": item.y = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
            //        case "rot": item.rot = ReaderUtil.ConvertSingle(attribute.Value, 0.0f); break;
            //    }
            //}
            return item;
        }
    }
    #endregion

    #region boardarray
    public class ElementBoardArrays
    {
        public const string Name = "boardarrays";
        public ElementBoardArray[] Items = new ElementBoardArray[] { };

        public ElementBoardArrays() { }

        public ElementBoardArray ElementBoardArray
        {
            get => default;
            set
            {
            }
        }

        public static ElementBoardArrays Create(XmlReader element)
        {
            if (element == null)
                return null;

            ElementBoardArrays item = new ElementBoardArrays();

            List<ElementBoardArray> newItems = new List<ElementBoardArray>();
            if (!element.IsEmptyElement)
                while (element.Read())
                {
                    if (element.NodeType == XmlNodeType.Whitespace)
                        continue;
                    if (element.NodeType == XmlNodeType.Element && string.Compare(element.Name, ElementBoardArray.Name, true) == 0)
                    {
                        ElementBoardArray newElement = ElementBoardArray.Create(element);
                        newItems.Add(newElement);

                    }
                    else if (element.NodeType == XmlNodeType.EndElement && string.Compare(element.Name, ElementBoardArrays.Name, true) == 0)
                        break;
                }
            item.Items = newItems.ToArray();

            //item.Items
            //    = (from Element in element.Elements(ElementBoardArray.Name)
            //       select ElementBoardArray.Create(Element)
            //           ).ToArray();
            return item;
        }
    }
    public class ElementBoardArray
    {
        public const string Name = "boardarray";

        public int num = 0;
        public string name = @"";
        public float x = 0.0f;
        public float y = 0.0f;
        public float orgx = 0.0f;
        public float orgy = 0.0f;
        public float rot = 0.0f;
        public bool exec = false;
        public int group = 0;


        public ElementBoardArray() { }

        public static ElementBoardArray Create(XmlReader element)
        {
            if (element == null)
                return null;

            ElementBoardArray item = new ElementBoardArray();

            if (element.NodeType == XmlNodeType.Element && element.HasAttributes)
            {
                while (element.MoveToNextAttribute())
                    switch (element.Name)
                    {
                        case "num": item.num = ReaderUtil.ConvertInt32(element.Value, 0); break;
                        case "name": item.name = element.Value; break;
                        case "x": item.x = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "y": item.y = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "orgx": item.orgx = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "orgy": item.orgy = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "rot": item.rot = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "bExec": item.exec = ReaderUtil.ConvertBoolean(element.Value, false); break;
                        case "group": item.group = ReaderUtil.ConvertInt32(element.Value, 0); break;
                    }
            }



            return item;
        }

    }
    #endregion


    #region boardarray
    public class ElementParts
    {
        public const string Name = "parts";
        public Dictionary<string, ElementPart> ItemDic = new Dictionary<string, ElementPart>();
        public ElementParts() { }

        public ElementPart ElementPart
        {
            get => default;
            set
            {
            }
        }

        public static ElementParts Create(XmlReader element)
        {
            if (element == null)
                return null;

            ElementParts item = new ElementParts();
            if (!element.IsEmptyElement)
                while (element.Read())
                {
                    if (element.NodeType == XmlNodeType.Whitespace)
                        continue;
                    if (element.NodeType == XmlNodeType.Element && string.Compare(element.Name, ElementPart.Name, true) == 0)
                    {
                        ElementPart newElement = ElementPart.Create(element);
                        item.ItemDic.Add(newElement.name, newElement);

                    }
                    else if (element.NodeType == XmlNodeType.EndElement && string.Compare(element.Name, ElementParts.Name, true) == 0)
                        break;
                }

            //item.ItemDic
            //    = (from Element in element.Elements(ElementPart.Name)
            //       select ElementPart.Create(Element)
            //           ).ToDictionary(x => x.name);
            return item;
        }
    }
    public class ElementPart
    {
        public const string Name = "part";

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

        public ElementPackages.ElementPackage ElementPackage
        {
            get => default;
            set
            {
            }
        }

        public static ElementPart Create(XmlReader element)
        {
            if (element == null)
                return null;

            ElementPart item = new ElementPart();

            if (element.NodeType == XmlNodeType.Element && element.HasAttributes)
            {
                while (element.MoveToNextAttribute())
                    switch (element.Name)
                    {
                        case "name": item.name = element.Value; break;
                        case "AlterPart": item.AlterPart = element.Value; break;
                        case "uid": item.uid = ReaderUtil.ConvertGuid(element.Value, Guid.Empty); break;
                        case "footprint": item.footprint = element.Value; break;
                        case "pkg": item.pkg = element.Value; break;
                        case "orgPkgName": item.orgPkgName = element.Value; break;
                        case "xoffset": item.xoffset = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "yoffset": item.yoffset = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "OffsetAngForUsrRefAng":
                            item.OffsetAngForUsrRefAng = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                    }
            }
            return item;
        }
    }
    #endregion

    public class ElementComponents
    {
        public const string Name = "components";
        public ElementComponent[] component = new ElementComponent[] { };

        public ElementComponents() { }

        public ElementComponent ElementComponent
        {
            get => default;
            set
            {
            }
        }

        public static ElementComponents Create(XmlReader element)
        {
            if (element == null)
                return null;

            ElementComponents item = new ElementComponents();

            List<ElementComponent> newItems = new List<ElementComponent>();
            if (!element.IsEmptyElement)
                while (element.Read())
                {
                    if (element.NodeType == XmlNodeType.Whitespace)
                        continue;

                    if (element.NodeType == XmlNodeType.Element && string.Compare(element.Name, ElementComponent.Name, true) == 0)
                    {
                        ElementComponent newElement = ElementComponent.Create(element);
                        newItems.Add(newElement);

                    }
                    else if (element.NodeType == XmlNodeType.EndElement && string.Compare(element.Name, ElementComponents.Name, true) == 0)
                        break;
                }
            item.component = newItems.ToArray();


            //item.component
            //    = (from Element in element.Elements(ElementComponent.Name)
            //       select ElementComponent.Create(Element)
            //           ).ToArray();
            return item;
        }
    }
    public class ElementComponent
    {
        public const string Name = "component";

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


        public ElementPart elPart = null;
        public ElementFootprint elFootprint = null;

        public ElementComponent() { }

        public ElementFootprint ElementFootprint
        {
            get => default;
            set
            {
            }
        }

        public static ElementComponent Create(XmlReader element)
        {
            if (element == null)
                return null;

            ElementComponent item = new ElementComponent();


            if (element.NodeType == XmlNodeType.Element && element.HasAttributes)
            {
                while (element.MoveToNextAttribute())
                    switch (element.Name)
                    {
                        case "name": item.name = element.Value; break;
                        case "userName": item.userName = element.Value; break;
                        case "bExec": item.bExec = ReaderUtil.ConvertBoolean(element.Value, false); break;
                        case "group": item.group = ReaderUtil.ConvertInt32(element.Value, 0); break;
                        case "Filter": item.Filter = ReaderUtil.ConvertBoolean(element.Value, false); break;
                        case "bOriExec": item.bOriExec = ReaderUtil.ConvertBoolean(element.Value, false); break;
                        case "part": item.part = element.Value; break;
                        case "partguid": item.partguid = ReaderUtil.ConvertGuid(element.Value, Guid.Empty); break;
                        case "OrgPart": item.OrgPart = element.Value; break;
                        case "AlterPart": item.AlterPart = element.Value; break;
                        case "copyPackage": item.copyPackage = element.Value; break;
                        case "si": item.si = element.Value; break;
                        case "x": item.x = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "y": item.y = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "rot": item.rot = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "fAngleByCompItSelf": item.fAngleByCompItSelf = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "bModifiedForBadComp": item.bModifiedForBadComp = ReaderUtil.ConvertBoolean(element.Value, false); break;
                        case "bDontAttachPkg": item.bDontAttachPkg = ReaderUtil.ConvertBoolean(element.Value, false); break;
                        case "inspGroupUid": item.inspGroupUid = element.Value; break;
                        case "bChkAbsence": item.bChkAbsence = ReaderUtil.ConvertBoolean(element.Value, false); break;
                        case "fAngleForInsp": item.fAngleForInsp = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "fAngleCutomerOrg": item.fAngleCutomerOrg = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "footprint": item.footprint = element.Value; break;
                        case "fovPosType": item.fovPosType = ReaderUtil.ConvertInt32(element.Value, 0); break;
                        case "OrgfovPosType": item.OrgfovPosType = ReaderUtil.ConvertInt32(element.Value, 0); break;
                        case "fovX": item.fovX = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "fovY": item.fovY = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "nCompPkgVerification": item.nCompPkgVerification = ReaderUtil.ConvertInt32(element.Value, 0); break;
                        case "nCompInspVerification": item.nCompInspVerification = ReaderUtil.ConvertInt32(element.Value, 0); break;
                        case "beforeDivideCenterX": item.beforeDivideCenterX = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "beforeDivideCenterY": item.beforeDivideCenterY = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "bDivided": item.bDivided = ReaderUtil.ConvertBoolean(element.Value, false); break;
                        case "beforeDivideCompName": item.beforeDivideCompName = element.Value; break;
                        case "beforeDivideBodyCenterX": item.beforeDivideBodyCenterX = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "beforeDivideBodyCenterY": item.beforeDivideBodyCenterY = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                    }
            }


            return item;
        }
    }
    /// <summary>
    /// pkg
    /// </summary>
    public class ElementPackages
    {
        public const string Name = "pkgs";
        //public ElementPackage[] Items;
        public Dictionary<string, ElementPackage> ItemDic = new Dictionary<string, ElementPackage>();
        public ElementPackages() { }

        public static ElementPackages Create(XmlReader element)
        {
            if (element == null)
                return null;

            ElementPackages item = new ElementPackages();
            if (!element.IsEmptyElement)
                while (element.Read())
                {
                    if (element.NodeType == XmlNodeType.Whitespace)
                        continue;
                    if (element.NodeType == XmlNodeType.Element && string.Compare(element.Name, ElementPackage.Name, true) == 0)
                    {
                        ElementPackage newElement = ElementPackage.Create(element);
                        item.ItemDic.Add(newElement.name, newElement);

                    }
                    else if (element.NodeType == XmlNodeType.EndElement && string.Compare(element.Name, ElementPackages.Name, true) == 0)
                        break;
                }

            //item.ItemDic
            //    = (from Element in element.Elements(ElementPackage.Name)
            //       select ElementPackage.Create(Element)
            //           ).ToDictionary(x => x.name);

            return item;
        }
        public class ElementPackage
        {
            public const string Name = "pkg";
            public ElementPackageBody[] pkgbody;
            public ElementPackagePins[] pkgpins;
            //<pkg 
            public string name;// = "277371" 
            public Guid uid;//="{589266CF-C956-4BE3-86DA-43305E7A3586}" 
            public string type;//="CAPACITOR" 
            public string pkgInspGroup;//="Standards" 
            public string maker;//="" 
            public int pkgApplyLevel;//="2" 
            public int fOrgOffsetAng;//="-999">

            public ElementPackage() { }

            public static ElementPackage Create(XmlReader element)
            {
                if (element == null)
                    return null;

                ElementPackage item = new ElementPackage();
                if (element.NodeType == XmlNodeType.Element && element.HasAttributes)
                {
                    while (element.MoveToNextAttribute())
                        switch (element.Name)
                        {
                            case "name": item.name = element.Value; break;
                            case "uid": item.uid = ReaderUtil.ConvertGuid(element.Value, Guid.Empty); break;
                            case "type": item.type = element.Value; break;
                            case "pkgInspGroup": item.pkgInspGroup = element.Value; break;
                            case "maker": item.maker = element.Value; break;
                            case "pkgApplyLevel": item.pkgApplyLevel = ReaderUtil.ConvertInt32(element.Value, 0); break;
                            case "fOrgOffsetAng": item.fOrgOffsetAng = ReaderUtil.ConvertInt32(element.Value, 0); break;
                        }
                }


                List<ElementPackageBody> newBodyItems = new List<ElementPackageBody>();
                List<ElementPackagePins> newPinItems = new List<ElementPackagePins>();
                if (!element.IsEmptyElement)
                    while (element.Read())
                    {
                        if (element.NodeType == XmlNodeType.Whitespace)
                            continue;
                        if (element.NodeType == XmlNodeType.Element && string.Compare(element.Name, ElementPackageBody.Name, true) == 0)
                        {
                            ElementPackageBody newElement = ElementPackageBody.Create(element);
                            newBodyItems.Add(newElement);

                        }
                        else if (element.NodeType == XmlNodeType.Element && string.Compare(element.Name, ElementPackagePins.Name, true) == 0)
                        {
                            ElementPackagePins newElement = ElementPackagePins.Create(element);
                            newPinItems.Add(newElement);

                        }
                        else if (element.NodeType == XmlNodeType.EndElement && string.Compare(element.Name, ElementPackage.Name, true) == 0)
                            break;
                    }
                item.pkgbody = newBodyItems.ToArray();
                item.pkgpins = newPinItems.ToArray();



                //item.pkgbody
                //    = (from Element in element.Elements(ElementPackageBody.Name)
                //       select ElementPackageBody.Create(Element)
                //           ).ToArray();
                //item.pkgpins
                //    = (from Element in element.Elements(ElementPackagePins.Name)
                //       select ElementPackagePins.Create(Element)
                //           ).ToArray();
                return item;
            }

            public class ElementPackageBody
            {
                public const string Name = "pkgbody";
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
                public static ElementPackageBody Create(XmlReader element)
                {
                    if (element == null)
                        return null;
                    ElementPackageBody item = new ElementPackageBody();

                    if (element.NodeType == XmlNodeType.Element && element.HasAttributes)
                    {
                        while (element.MoveToNextAttribute())
                            switch (element.Name)
                            {
                                case "shuid": item.shuid = ReaderUtil.ConvertGuid(element.Value, Guid.Empty); break;
                                case "x": item.x = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                                case "y": item.y = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                                case "rot": item.rot = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                                case "thickness": item.thickness = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                                case "gap": item.gap = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                                case "wptp": item.wptp = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                                case "hptp": item.hptp = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                                case "pitch": item.pitch = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                            }
                    }
                    return item;
                }
            }

            public class ElementPackagePins
            {
                public const string Name = "pkgpins";
                public ElementPackagePin[] pkgpin;

                public ElementPackagePins() { }
                public static ElementPackagePins Create(XmlReader element)
                {
                    if (element == null)
                        return null;

                    ElementPackagePins item = new ElementPackagePins();
                    List<ElementPackagePin> newItems = new List<ElementPackagePin>();
                    if (!element.IsEmptyElement)
                        while (element.Read())
                        {
                            if (element.NodeType == XmlNodeType.Whitespace)
                                continue;
                            if (element.NodeType == XmlNodeType.Element && string.Compare(element.Name, ElementPackagePin.Name, true) == 0)
                            {
                                ElementPackagePin newElement = ElementPackagePin.Create(element);
                                newItems.Add(newElement);

                            }
                            else if (element.NodeType == XmlNodeType.EndElement && string.Compare(element.Name, ElementPackagePins.Name, true) == 0)
                                break;
                        }
                    item.pkgpin = newItems.ToArray();

                    //item.pkgpin
                    //    = (from Element in element.Elements(ElementPackagePin.Name)
                    //       select ElementPackagePin.Create(Element)
                    //           ).ToArray();

                    return item;
                }

                public class ElementPackagePin
                {
                    public const string Name = "pkgpin";
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
                    public static ElementPackagePin Create(XmlReader element)
                    {
                        if (element == null)
                            return null;
                        ElementPackagePin item = new ElementPackagePin();

                        if (element.NodeType == XmlNodeType.Element && element.HasAttributes)
                        {
                            while (element.MoveToNextAttribute())
                                switch (element.Name)
                                {
                                    case "name": item.name = element.Value; break;
                                    case "userName": item.userName = element.Value; break;
                                    case "uid": item.uid = ReaderUtil.ConvertGuid(element.Value, Guid.Empty); break;
                                    case "order": item.order = ReaderUtil.ConvertInt32(element.Value, 0); break;
                                    case "shuid": item.shuid = ReaderUtil.ConvertGuid(element.Value, Guid.Empty); break;
                                    case "x": item.x = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                                    case "y": item.y = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                                    case "rot": item.rot = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                                    case "thickness": item.thickness = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                                    case "type": item.type = element.Value; break;
                                    case "attachSide": item.attachSide = element.Value; break;
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
        public const string Name = "pkgBodyShapes";
        //public ElementShape[] shape;
        public Dictionary<Guid, ElementShape> ItemDic = new Dictionary<Guid, ElementShape>();
        public ElementPackageBodyShapes() { }

        public ElementShape ElementShape
        {
            get => default;
            set
            {
            }
        }

        public static ElementPackageBodyShapes Create(XmlReader element)
        {
            if (element == null)
                return null;

            ElementPackageBodyShapes item = new ElementPackageBodyShapes();
            if (!element.IsEmptyElement)
                while (element.Read())
                {
                    if (element.NodeType == XmlNodeType.Whitespace)
                        continue;
                    if (element.NodeType == XmlNodeType.Element && string.Compare(element.Name, ElementShape.Name, true) == 0)
                    {
                        ElementShape newElement = ElementShape.Create(element);
                        item.ItemDic.Add(newElement.uid, newElement);

                    }
                    else if (element.NodeType == XmlNodeType.EndElement && string.Compare(element.Name, ElementPackageBodyShapes.Name, true) == 0)
                        break;
                }

            //item.ItemDic
            //    = (from Element in element.Elements(ElementShape.Name)
            //       select ElementShape.Create(Element)
            //           ).ToDictionary(x => x.uid);
            return item;
        }
    }
    public class ElementPackageLeadShapes
    {
        public const string Name = "pkgLeadShapes";
        //public ElementShape[] shape;
        public Dictionary<Guid, ElementShape> ItemDic = new Dictionary<Guid, ElementShape>();
        public ElementPackageLeadShapes() { }

        public ElementShape ElementShape
        {
            get => default;
            set
            {
            }
        }

        public static ElementPackageLeadShapes Create(XmlReader element)
        {
            if (element == null)
                return null;

            ElementPackageLeadShapes item = new ElementPackageLeadShapes();
            if (!element.IsEmptyElement)
                while (element.Read())
                {
                    if (element.NodeType == XmlNodeType.Whitespace)
                        continue;
                    if (element.NodeType == XmlNodeType.Element && string.Compare(element.Name, ElementShape.Name, true) == 0)
                    {
                        ElementShape newElement = ElementShape.Create(element);
                        item.ItemDic.Add(newElement.uid, newElement);

                    }
                    else if (element.NodeType == XmlNodeType.EndElement && string.Compare(element.Name, ElementPackageLeadShapes.Name, true) == 0)
                        break;
                }
            //item.ItemDic
            //    = (from Element in element.Elements(ElementShape.Name)
            //       select ElementShape.Create(Element)
            //           ).ToDictionary(x => x.uid);
            return item;
        }
    }
    public class ElementFootprintLeadShapes
    {
        public const string Name = "footprtLeadShapes";
        //public ElementShape[] shape;
        public Dictionary<Guid, ElementShape> ItemDic = new Dictionary<Guid, ElementShape>();

        public ElementFootprintLeadShapes() { }

        public ElementShape ElementShape
        {
            get => default;
            set
            {
            }
        }

        public static ElementFootprintLeadShapes Create(XmlReader element)
        {
            if (element == null)
                return null;

            ElementFootprintLeadShapes item = new ElementFootprintLeadShapes();
            if (!element.IsEmptyElement)
                while (element.Read())
                {
                    if (element.NodeType == XmlNodeType.Whitespace)
                        continue;
                    if (element.NodeType == XmlNodeType.Element && string.Compare(element.Name, ElementShape.Name, true) == 0)
                    {
                        ElementShape newElement = ElementShape.Create(element);
                        item.ItemDic.Add(newElement.name, newElement);

                    }
                    else if (element.NodeType == XmlNodeType.EndElement && string.Compare(element.Name, ElementFootprintLeadShapes.Name, true) == 0)
                        break;
                }
            //item.ItemDic
            //    = (from Element in element.Elements(ElementShape.Name)
            //       select ElementShape.Create(Element)
            //           ).ToDictionary(x => x.uid);
            return item;
        }
    }

    public class ElementShape
    {
        public const string Name = "shape";
        public ElementRect[] rc;
        public ElementDrawPath[] drawpath;

        public Guid uid;//="{03400597-2365-47CD-B8EF-DA7BC634B0E3}" 
        public Guid name;//="{03400597-2365-47CD-B8EF-DA7BC634B0E3}"


        public ElementShape() { }

        public ElementRect ElementRect
        {
            get => default;
            set
            {
            }
        }

        public ElementDrawPath ElementDrawPath
        {
            get => default;
            set
            {
            }
        }

        public static ElementShape Create(XmlReader element)
        {

            ElementShape item = new ElementShape();

            if (element.NodeType == XmlNodeType.Element && element.HasAttributes)
            {
                while (element.MoveToNextAttribute())
                    switch (element.Name)
                    {
                        case "uid": item.uid = ReaderUtil.ConvertGuid(element.Value, Guid.Empty); break;
                        case "name": item.name = ReaderUtil.ConvertGuid(element.Value, Guid.Empty); break;
                    }
            }

            List<ElementRect> newRcItems = new List<ElementRect>();
            List<ElementDrawPath> newDrawPathItems = new List<ElementDrawPath>();
            if (!element.IsEmptyElement)
                while (element.Read())
                {
                    if (element.NodeType == XmlNodeType.Whitespace)
                        continue;
                    if (element.NodeType == XmlNodeType.Element && string.Compare(element.Name, ElementRect.Name, true) == 0)
                    {
                        ElementRect newElement = ElementRect.Create(element);
                        newRcItems.Add(newElement);

                    }
                    else if (element.NodeType == XmlNodeType.Element && string.Compare(element.Name, ElementDrawPath.Name, true) == 0)
                    {
                        ElementDrawPath newElement = ElementDrawPath.Create(element);
                        newDrawPathItems.Add(newElement);

                    }
                    else if (element.NodeType == XmlNodeType.EndElement && string.Compare(element.Name, ElementShape.Name, true) == 0)
                        break;
                }
            item.rc = newRcItems.ToArray();
            item.drawpath = newDrawPathItems.ToArray();



            //item.rc
            //    = (from Element in element.Elements(ElementRect.Name)
            //       select ElementRect.Create(Element)
            //           ).ToArray();
            //item.drawpath
            //    = (from Element in element.Elements(ElementDrawPath.Name)
            //       select ElementDrawPath.Create(Element)
            //           ).ToArray();

            return item;
        }


    }
    public class ElementDrawPath
    {
        public const string Name = "drawpath";
        ElementPoint[] pt;
        float area;

        public ElementDrawPath() { }

        public ElementPoint ElementPoint
        {
            get => default;
            set
            {
            }
        }

        public static ElementDrawPath Create(XmlReader element)
        {
            if (element == null)
                return null;

            ElementDrawPath item = new ElementDrawPath();

            if (element.NodeType == XmlNodeType.Element && element.HasAttributes)
            {
                while (element.MoveToNextAttribute())
                    switch (element.Name)
                    {
                        case "area": item.area = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                    }
            }

            List<ElementPoint> newItems = new List<ElementPoint>();
            if (!element.IsEmptyElement)
                while (element.Read())
                {
                    if (element.NodeType == XmlNodeType.Whitespace)
                        continue;
                    if (element.NodeType == XmlNodeType.Element && string.Compare(element.Name, ElementPoint.Name, true) == 0)
                    {
                        ElementPoint newElement = ElementPoint.Create(element);
                        newItems.Add(newElement);

                    }
                    else if (element.NodeType == XmlNodeType.EndElement && string.Compare(element.Name, ElementDrawPath.Name, true) == 0)
                        break;
                }
            item.pt = newItems.ToArray();

            //item.pt
            //    = (from Element in element.Elements(ElementPoint.Name)
            //       select ElementPoint.Create(Element)
            //           ).ToArray();
            return item;
        }
    }
    public class ElementRect
    {
        public const string Name = "rc";
        public float width;// = "0.20000"
        public float length;// ="0.40000"
        public float w;// = "0.40000"
        public float h;// = "0.20000"
        public float rot;// = "0.00000"
        public float area;// = "0.000000"
        public ElementRect() { }
        public static ElementRect Create(XmlReader element)
        {
            if (element == null)
                return null;

            ElementRect item = new ElementRect();

            if (element.NodeType == XmlNodeType.Element && element.HasAttributes)
            {
                while (element.MoveToNextAttribute())
                    switch (element.Name)
                    {
                        case "width": item.width = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "length": item.length = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "w": item.w = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "h": item.h = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "rot": item.rot = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "area": item.area = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                    }
            }

            return item;
        }
    }
    public class ElementPoint
    {
        public const string Name = "pt";
        float x;
        float y;
        public ElementPoint() { }
        public static ElementPoint Create(XmlReader element)
        {
            if (element == null)
                return null;

            ElementPoint item = new ElementPoint();

            if (element.NodeType == XmlNodeType.Element && element.HasAttributes)
            {
                while (element.MoveToNextAttribute())
                    switch (element.Name)
                    {
                        case "x": item.x = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "y": item.y = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                    }
            }
            return item;
        }
    }

    public class ElementFovs
    {
        public const string Name = "fovs";
        public ElementFov[] fovs;

        public ElementFovs() { }

        public ElementFov ElementFov
        {
            get => default;
            set
            {
            }
        }

        public static ElementFovs Create(XmlReader element)
        {
            if (element == null)
                return null;

            ElementFovs item = new ElementFovs();

            List<ElementFov> newItems = new List<ElementFov>();
            if (!element.IsEmptyElement)
                while (element.Read())
                {
                    if (element.NodeType == XmlNodeType.Whitespace)
                        continue;
                    if (element.NodeType == XmlNodeType.Element && string.Compare(element.Name, ElementFov.Name, true) == 0)
                    {
                        ElementFov newElement = ElementFov.Create(element);
                        newItems.Add(newElement);

                    }
                    else if (element.NodeType == XmlNodeType.EndElement && string.Compare(element.Name, ElementFovs.Name, true) == 0)
                        break;
                }
            item.fovs = newItems.ToArray();

            //item.fovs

            //    = (from Element in element.Elements(ElementFov.Name)
            //       select ElementFov.Create(Element)
            //           ).ToArray();
            return item;
        }
    }
    public class ElementBKFovs
    {
        public const string Name = "bkfovs";
        public ElementFov[] fovs;

        public ElementBKFovs() { }

        public ElementFov ElementFov
        {
            get => default;
            set
            {
            }
        }

        public static ElementBKFovs Create(XmlReader element)
        {
            if (element == null)
                return null;

            ElementBKFovs item = new ElementBKFovs();
            List<ElementFov> newItems = new List<ElementFov>();
            if (!element.IsEmptyElement)
                while (element.Read())
                {
                    if (element.NodeType == XmlNodeType.Whitespace)
                        continue;
                    if (element.NodeType == XmlNodeType.Element && string.Compare(element.Name, ElementFov.Name, true) == 0)
                    {
                        ElementFov newElement = ElementFov.Create(element);
                        newItems.Add(newElement);

                    }
                    else if (element.NodeType == XmlNodeType.EndElement && string.Compare(element.Name, ElementBKFovs.Name, true) == 0)
                        break;
                }
            item.fovs = newItems.ToArray();
            //item.fovs

            //    = (from Element in element.Elements(ElementFov.Name)
            //       select ElementFov.Create(Element)
            //           ).ToArray();
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

        public ElementFov ElementFov
        {
            get => default;
            set
            {
            }
        }

        public static ElementBoardFovs Create(XmlReader element)
        {
            if (element == null)
                return null;

            ElementBoardFovs item = new ElementBoardFovs();

            if (element.NodeType == XmlNodeType.Element && element.HasAttributes)
            {
                while (element.MoveToNextAttribute())
                    switch (element.Name)
                    {
                        case "width": item.width = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "height": item.height = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                    }
            }

            List<ElementFov> newItems = new List<ElementFov>();
            if (!element.IsEmptyElement)
                while (element.Read())
                {
                    if (element.NodeType == XmlNodeType.Whitespace)
                        continue;
                    if (element.NodeType == XmlNodeType.Element && string.Compare(element.Name, ElementFov.Name, true) == 0)
                    {
                        ElementFov newElement = ElementFov.Create(element);
                        newItems.Add(newElement);

                    }
                    else if (element.NodeType == XmlNodeType.EndElement && string.Compare(element.Name, ElementBoardFovs.Name, true) == 0)
                        break;
                }
            item.fovs = newItems.ToArray();

            return item;
        }
    }

    public class ElementFov
    {
        public const string Name = "fov";
        public int id;// = "0.20000"
        public float left;// ="0.40000"
        public float height;// = "0.40000"
        public float x;
        public float y;

        public ElementFov() { }
        public static ElementFov Create(XmlReader element)
        {
            if (element == null)
                return null;

            ElementFov item = new ElementFov();

            if (element.NodeType == XmlNodeType.Element && element.HasAttributes)
            {
                while (element.MoveToNextAttribute())
                    switch (element.Name)
                    {
                        case "id": item.id = ReaderUtil.ConvertInt32(element.Value, 0); break;
                        case "left": item.left = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "height": item.height = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "x": item.x = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
                        case "y": item.y = ReaderUtil.ConvertSingle(element.Value, 0.0f); break;
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
            if (string.IsNullOrEmpty(Value))
                return Default;

            if (
            System.Text.RegularExpressions.Regex.IsMatch(Value
                , @"(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}"))
            {
                return new Guid(Value);
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