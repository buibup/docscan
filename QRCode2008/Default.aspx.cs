using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Drawing;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.Web;
using System.Threading;
using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Ecc;
using MessagingToolkit.QRCode.Codec.Data;
using MessagingToolkit.QRCode.Codec.Util;
using System.IO;
using System.Data.Odbc;


namespace DocScanEPR
{
    public partial class _Default : System.Web.UI.Page
    {
        private clstDataManager ODataManager = new clstDataManager();
        MessagingToolkit.QRCode.Codec.QRCodeEncoder qe = new MessagingToolkit.QRCode.Codec.QRCodeEncoder();
        ReportDocument cryRpt = new ReportDocument();
        private string DocNameT;
        DataTable DTConsen = new DataTable();

        #region GetTypeDoc
        private string GetTypeDoc(string DocCallName) // Load DocType for Create QRCode--------
        {
            string DocType = "";
            DataTable DTDoc = new DataTable();
            if (DocCallName != "")
            {
                try
                {
                    //DTDoc = ODataManager.GetDataSQL("select  DocTtype,DocName  from  ConsentFromRef  where  DocCallName ='" + DocName + "'");
                    DTDoc = ODataManager.GetDataSQL("select  *  from  ConsentFromRef  where  DocCallName ='" + DocCallName + "'");
                    DTConsen = DTDoc;
                    if (DTDoc.Rows.Count > 0)
                    {
                        if (DTDoc.Rows[0]["DocTtype"] != null)
                        {
                            DocType = DTDoc.Rows[0]["DocTtype"].ToString().Trim();
                        }

                        if (DTDoc.Rows[0]["DocName"] != null)
                        {
                            DocNameT = DTDoc.Rows[0]["DocName"].ToString().Trim();
                        }

                    }
                }
                catch { }
            }

            return DocType;
        }
        #endregion

        #region GetAllergy
        private string GetAlg(int Papmi_RowID)
        {
            DataTable dt = new DataTable();
            String AL = "";
            String ALPHCGE = "";
            String ALComments = "";
            String ALDesc = "";
            String ALPHCD = "";
            String ALALGR_Desc = "";
            int i = 0;
            dt = ODataManager.GetData("select PHCD_Name,PHCGE_Name,ALG_Comments,ALG_Desc,ALGR_Desc  from VSVH_ALG  where papmi_rowid = '" + Papmi_RowID + "'  and  AllergyStatus <>'I'");
            if (dt.Rows.Count > 0)
            {
                for (i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["ALG_Desc"] != null)
                    {
                        if (dt.Rows[i]["ALG_Desc"].ToString().Trim() != "")
                        {
                            if (ALDesc != "")
                            {
                                ALDesc = ALDesc + "," + dt.Rows[i]["ALG_Desc"].ToString();
                            }
                            else
                            {
                                ALDesc = dt.Rows[i]["ALG_Desc"].ToString();
                            }


                        }


                    }

                    if (dt.Rows[i]["ALGR_Desc"] != null)
                    {
                        if (dt.Rows[i]["ALGR_Desc"].ToString().Trim() != "")
                        {
                            if (ALALGR_Desc != "")
                            {
                                ALALGR_Desc = ALALGR_Desc + "," + dt.Rows[i]["ALGR_Desc"].ToString();
                            }
                            else
                            {
                                ALALGR_Desc = dt.Rows[i]["ALGR_Desc"].ToString();
                            }


                        }


                    }

                    if (dt.Rows[i]["PHCGE_Name"] != null)
                    {
                        if (dt.Rows[i]["PHCGE_Name"].ToString().Trim() != "")
                        {
                            if (ALPHCGE != "")
                            {
                                ALPHCGE = ALPHCGE + "," + dt.Rows[i]["PHCGE_Name"].ToString();
                            }
                            else
                            {
                                ALPHCGE = dt.Rows[i]["ALG_Desc"].ToString();
                            }


                        }


                    }


                    if (dt.Rows[i]["ALG_Comments"] != null)
                    {
                        if (dt.Rows[i]["ALG_Comments"].ToString().Trim() != "")
                        {
                            if (ALComments != "")
                            {
                                ALComments = ALComments + "," + dt.Rows[i]["ALG_Comments"].ToString();
                            }
                            else
                            {
                                ALComments = dt.Rows[i]["ALG_Comments"].ToString();
                            }


                        }


                    }

                    if (dt.Rows[i]["PHCD_Name"] != null)
                    {
                        if (dt.Rows[i]["PHCD_Name"].ToString().Trim() != "")
                        {
                            if (ALPHCD != "")
                            {
                                ALPHCD = ALPHCD + "," + dt.Rows[i]["PHCD_Name"].ToString();
                            }
                            else
                            {
                                ALPHCD = dt.Rows[i]["PHCD_Name"].ToString();
                            }


                        }


                    }

                }

            }


            //PHCGE_Name+ALGR_Desc+PHCD_Name+StrAllergyComment+ALG_Desc
            if (ALPHCGE.Trim() != "")
            {
                AL = ALPHCGE;
            }
            if (ALALGR_Desc.Trim() != "")
            {
                if (AL.Trim() != "")
                {
                    AL = AL + "," + ALALGR_Desc;
                }
                else
                {
                    AL = ALALGR_Desc;
                }

            }
            if (ALPHCD.Trim() != "")
            {
                if (AL.Trim() != "")
                {
                    AL = AL + "," + ALPHCD;
                }
                else
                {
                    AL = ALPHCD;
                }

            }

            if (ALComments.Trim() != "")
            {
                if (AL.Trim() != "")
                {
                    AL = AL + "," + ALComments;
                }
                else
                {
                    AL = ALComments;
                }

            }
            if (ALDesc.Trim() != "")
            {
                if (AL.Trim() != "")
                {
                    AL = AL + "," + ALDesc;
                }
                else
                {
                    AL = ALDesc;
                }

            }

            if (AL == "") { AL = "  "; }
            return AL;

        }
        #endregion

        #region GetLocation Appointment
        public string GetLocApp(string EPIRowId,string CPRowId)
        {
            string loc = "";

            DataTable dt = new DataTable();
            if (CPRowId != "")
            {
                try
                {
                    dt = ODataManager.GetData("select CTLOC_AppLocDesc from vsvh_app2 where PAADM_RowID =  '" + EPIRowId + "' and  and CTPCP_AppRowId = '" + CPRowId + "' ");
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0]["CTLOC_AppLocDesc"] != null)
                        {
                            loc = dt.Rows[0]["CTLOC_AppLocDesc"].ToString();
                        }
                    }
                }
                catch
                {
                }
            }

            return loc;
        }
        #endregion

        #region GetMedicalLicenseNo
        public string GetMedLicenseNo(string CPRowId)
        {
            string SCMNO = "";

            DataTable dt = new DataTable();
            if (CPRowId != "")
            {
                try
                {
                    dt = ODataManager.GetData("select CTPCP_SMCNo from ct_careprov where ctpcp_rowid1 = '" + CPRowId + "' ");
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0]["CTPCP_SMCNo"] != null)
                        {
                            SCMNO = dt.Rows[0]["CTPCP_SMCNo"].ToString();
                        }
                    }
                }
                catch
                {
                }
            }

            return SCMNO;
        }
        #endregion

        #region GetEPIRowIdByQId
        public string GetEPIRowIdByQId(int QId)
        {
            string EPIRowId = "";
            DataTable DTQIns = new DataTable();
            if (QId != 0)
            {
                try
                {
                    DTQIns = ODataManager.GetData("select * from questionnaire.q01insmr where ID = " + QId + " ");
                    if (DTQIns.Rows.Count > 0)
                    {
                        if (DTQIns.Rows[0]["QUESPAAdmDR"] != null)
                        {
                            EPIRowId = DTQIns.Rows[0]["QUESPAAdmDR"].ToString();
                        }
                    }
                }
                catch
                {
                }
            }
            return EPIRowId;
        }
        #endregion

        private void OpenPDF(string downloadAsFilename)
        {
            ReportDocument Rel = new ReportDocument();
            Rel.Load(Server.MapPath("../Report/DoctorInsurance.rpt"));
            BinaryReader stream = new BinaryReader(Rel.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat));
            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment; filename=" + downloadAsFilename);
            Response.AddHeader("content-length", stream.BaseStream.Length.ToString());
            Response.BinaryWrite(stream.ReadBytes(Convert.ToInt32(stream.BaseStream.Length)));
            Response.Flush();
            Response.Close();
        }

        #region GetPath
        public string GetPath(string ReportName)
        {
            string Path = "";
            string PathName = "";
            DataTable DTReportName = new DataTable();
            if (ReportName != "")
            {
                try
                {
                    DTReportName = ODataManager.GetDataSQL("select  ReportPath  from  ConsentFromRef  where  DocCallName ='" + ReportName + "'");
                    if (DTReportName.Rows.Count > 0)
                    {
                        if (DTReportName.Rows[0]["ReportPath"] != null)
                        {
                            PathName = DTReportName.Rows[0]["ReportPath"].ToString();
                        }
                    }
                }
                catch { }

                if (PathName != "")
                {
                    //Path = Server.MapPath(PathName);
                    Path = PathName;
                }


            }
            return Path;
        }
        #endregion

        #region GetReport

        public void GetReport(string ReportName, int QID, string CPRowID)
        {
            ReportDocument report = new ReportDocument();
            string path = GetPath(ReportName);
            DataTable dt = DtGetConSentFormRefByReportName(ReportName);

            if (dt.Rows.Count > 0)
            {
                //report.Load(@"D:\Shared\SVN\DocScan\DocScanEPR\QRCode2008\Report\Patient And Family Education Recrord.rpt");
                //report.Load(Server.MapPath(path));

                report.Load(Server.MapPath(@"" + path + ""));
                report.FileName = Server.MapPath(@"" + path + "");
                if (dt.Rows[0]["ParameterName"] != null && dt.Rows[0]["ParameterName"].ToString() != "")
                {
                    report.SetParameterValue(dt.Rows[0]["ParameterName"].ToString(), QID);
                    if (dt.Rows[0]["ParameterName1"] != null && dt.Rows[0]["ParameterName1"].ToString() != "")
                    {
                        report.SetParameterValue(dt.Rows[0]["ParameterName1"].ToString(), CPRowID);
                    }
                }

            }
            else
            {
                report.Load(Server.MapPath(@"" + path + ""));
                report.FileName = Server.MapPath(@"" + path + "");
                report.SetParameterValue("QID", QID);
                report.SetParameterValue("DoctorID", CPRowID);
            }


            try
            {
                report.ExportToHttpResponse(ExportFormatType.PortableDocFormat, HttpContext.Current.Response, false, ReportName);
                report.Dispose();
            }
            catch (System.Exception ex)
            {
                Thread.ResetAbort();
            }
        }
        #endregion

        #region GetReport

        public void GetReportFlaxCliam(string ReportName, String Hn, string Room)
        {
            ReportDocument report = new ReportDocument();
            string path = GetPath(ReportName);
            DataTable dt = DtGetConSentFormRefByReportName(ReportName);

            if (dt.Rows.Count > 0)
            {

                report.Load(Server.MapPath(@"" + path + ""));
                report.FileName = Server.MapPath(@"" + path + "");
                if (dt.Rows[0]["ParameterName"] != null && dt.Rows[0]["ParameterName"].ToString() != "")
                {
                    report.SetParameterValue(dt.Rows[0]["ParameterName"].ToString(), Hn);
                    if (dt.Rows[0]["ParameterName1"] != null && dt.Rows[0]["ParameterName1"].ToString() != "")
                    {
                        report.SetParameterValue(dt.Rows[0]["ParameterName1"].ToString(), Room);
                    }
                }

            }
            else
            {
                report.Load(Server.MapPath(@"" + path + ""));
                report.FileName = Server.MapPath(@"" + path + "");
                report.SetParameterValue("Hn", Hn);
                report.SetParameterValue("Room", Room);
            }


            try
            {
                report.ExportToHttpResponse(ExportFormatType.PortableDocFormat, HttpContext.Current.Response, false, ReportName);
                report.Dispose();
            }
            catch (System.Exception ex)
            {
                Thread.ResetAbort();
            }
        }
        #endregion

        #region DtGetConSentFormRefByReportName
        public DataTable DtGetConSentFormRefByReportName(string reportName)
        {
            DataTable DTConS = new DataTable();
            DTConS = ODataManager.GetDataSQL("select  *  from  ConsentFromRef  where  DocCallName ='" + reportName + "'");

            return DTConS;
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            // string url2 = "http:/ /localhost:13482/Default.aspx?Paadm_RowId=5132358&PapmiRowId=5678&ReportName=SVHConsentForTh";
            string EpisodeRowId = "";
            string ReportName = "";
            int QID = 0;
            string AppID = "";
            string Hn = "";
            string Room = "";
            bool HasAppID = false;
            string CPRowID = "";
            bool HasCPRowID = false;

            #region RequestQueryStrin
            //======================================================
            if (Request.QueryString["Paadm_RowId"] != null)
            {
                EpisodeRowId = Request.QueryString["Paadm_RowId"].ToString();
            }

            if (Request.QueryString["ReportName"] != null)
            {
                ReportName = Request.QueryString["ReportName"].ToString();
            }

            if (Request.QueryString["QID"] != null)
            {
                if (Request.QueryString["QID"].Trim() != "")
                {
                    QID = Convert.ToInt32(Request.QueryString["QID"]);
                }
            }

            if (Request.QueryString["ApptID"] != null)
            {
                HasAppID = true;
                AppID = Request.QueryString["ApptID"].ToString();
            }

            if (Request.QueryString["cprowid"] != null)
            {
                HasCPRowID = true;
                CPRowID = Request.QueryString["cprowid"].ToString();
            }

            if (Request.QueryString["Hn"] != null)
            {

                Hn = Request.QueryString["Hn"].ToString();
            }

            if (Request.QueryString["Room"] != null)
            {

                Room = Request.QueryString["Room"].ToString();
            }
            //======================================================
            #endregion



            #region TestReport


            ReportName = "PatientAndFamilyEducationRecrord";
            QID = 42711;

            //11-MRF-15-07  หนังสือแสดงความยินยอมรับ การผ่าตัด
            //ReportName = "11-MRF-15-07";
            //EpisodeRowId = "14473310";

            //DoctorInsurance
            //ReportName = "DoctorInsurance";
            //QID = 21;
            //EpisodeRowId = "15056432";

            //EpisodeRowId = "14473310";
            //QID = 43214;

            //ReportName = "OPD CLAIM FORM SNH";
            //OPD CLAIM FORM SNH 14473310,43214 : 13969429,23901

            //ReportName = "ALGTest";
            //EpisodeRowId = "14412586";

            //ReportName = "12-MRF-8-03";
            //EpisodeRowId = "13219385";

            //AdmissionNote
            //EpisodeRowId = "14393174";
            //QID = 1;
            //CPRowID = "1";
            //ReportName = "AdmissionNote";

            //AdmissionNotePed
            //QID = 1;
            //ReportName = "AdmissionNotePed";

            //EpisodeRowId = "14393174";
            //QID = 1;
            //ReportName = "11-mrf-488";

            //QRCodeStickerLAA
            //HasAppID = true;
            //EpisodeRowId = "14046076";
            //AppID = "13860";
            //ReportName = "QRCodeStickerLAA";

            //SVNHEPRDoctor
            //EpisodeRowId = "14333302";
            //QID = 1740;
            //ReportName = "SVNHEPRDoctor";

            //SVNHEPR-Audit
            //Paadm_RowId=15129142&ReportName=SVHEPR-Audit&QID=7344
            //EpisodeRowId = "15129142";
            //CPRowID = "7344";
            //ReportName = "SVHEPR-Audit";

            //EpisodeRowId = "15129142";
            //QID = 7344;
            //ReportName = "SVHEPR-Audit";

            //DoctorOrderSheet
            //EpisodeRowId = "14222843";
            //ReportName = "DoctorOrderSheet";

            //EpisodeRowId = "13304019";
            //ReportName = "Consent for Hemodialysis  12-MRF-154-01A";

            //test ANC
            //EpisodeRowId = "13304019";
            //ReportName = "ANC";
            //QID = 13;

            //test link cprowid
            //HasCPRowID = true;
            //CPRowID = "17050";
            //EpisodeRowId = "13948380";
            //ReportName = "11-MRF-47-02";

            //14046076
            //HasAppID = true;
            //EpisodeRowId = "14046076";
            //AppID = "13860";
            //ReportName = "Consent for Surgical Operation-Procedure (For Central Line) 11-MRF-15-05A";

            //EpisodeRowId = "14284484";
            //QID = 3;
            //ReportName = "MedcerSNH";
            //MedcerSVHENG 13858339,10231  14314932,37623
            //MedcerSNH 14284484,3

            //EpisodeRowId = "14473310";
            //QID = 43214;
            //ReportName = "OPD CLAIM FORM SNH";
            //OPD CLAIM FORM SNH 14473310,43214 : 13969429,23901
            //MedcerSNH
            //Consent for HIV Test 12-MRF-231A

            //13828996 9697

            //DataTable dte = new DataTable();
            //string ss = "";
            //ss = "{CALL \"Custom_THSV_Report_ZEN_StoredProc\".\"" + "SVNHDrScheduleByDR_getData" + "\".('11932143')}";

            //dte = ODataManager.CallStoredProcedure(ss);


            //EpisodeRowId = "13660035";
            //CPRowID = "11708";
            //ReportName = "SVNHEPRDoctorImage";

            //EpisodeRowId = "13660035";
            //CPRowID = "11708";
            //ReportName = "SVNHEPRDoctor";

            //SVNHEPR-ER PED

            //EpisodeRowId = "10377230";
            //QID = 15;
            //ReportName = "MedcerSNH_ENG";

            //EpisodeRowId = "16995234";
            //QID = 210833;
            //ReportName = "IPDCLAIMSNH_ENG";

            //EpisodeRowId = "13218182";
            //QID = 19;
            //ReportName = "MedcerSNH5ENG";
            //13218182
            //    19


            //Hn = "12-07-012737";
            //Room = "1234";
            ////QID = 19;
            //ReportName = "IPDSNH";

            #endregion

            string docType = "";
            docType = GetTypeDoc(ReportName);

            if (ReportName == "DoctorInsurance")
            {
                EpisodeRowId = GetEPIRowIdByQId(QID);
            }


            #region Check EpisodeRowId

            if (EpisodeRowId != "" && ReportName != "AdmissionNote" && ReportName != "" && docType != "admnte" && docType != "QN") //Check EPISODE 
            {

                // String Epi = "I11-08-013757";
                #region Create DataTable base from vs_admission
                DataTable dtTrak = new DataTable();
                DataTable dtApp = new DataTable();
                DataTable dtcprowid = new DataTable();
                dtTrak = ODataManager.GetData("select distinct CTPCP_SMCNo,CTCOU_Desc,CTOCC_Desc,CTRLG_Desc,CTMAR_Desc,CTPCP_StName,ROOM_Desc,WARD_Code,WARD_Desc,SSUSR_Name,PAPMI_No, PAPMI_DOB,CTPCP_Desc, CTSEX_Desc, PAPER_AgeYr,PAPER_AgeMth, CTLOC_Desc,PAADM_AdmDate,PAADM_ADMNo, PAPMI_Name3, PAPMI_Name2, PAPMI_Name, CTPCP_Code, CTLOC_Code,PAPMI_RowId,PAADM_RowID,PAPER_StNameLine1,CTCIT_Desc,CITAREA_Desc1,PROV_Desc,CTZIP_Code,PAPER_TelH,PAPER_Email,PAPER_Fax,PAPMI_Name8   from vs_admission  where PAADM_RowID=" + EpisodeRowId + "");
                // dtTrak = ODataManager.GetData("select distinct   CTPCP_StName,ROOM_Desc,WARD_Desc,SSUSR_Name,PAPMI_No, PAPMI_DOB,CTPCP_Desc, CTSEX_Desc, PAPER_AgeYr,PAPER_AgeMth, CTLOC_Desc,PAADM_AdmDate,PAADM_ADMNo, PAPMI_Name3, PAPMI_Name2, PAPMI_Name, CTPCP_Code, CTLOC_Code,PAPMI_RowId,PAADM_RowID,PAPER_StNameLine1,CTCIT_Desc,CITAREA_Desc1,PROV_Desc,CTZIP_Code,PAPER_TelH,PAPER_Email,PAPER_Fax   from vs_admission  where PAADM_RowID='" + EpisodeRowId + "'");
                try
                {
                    if (dtTrak.Rows.Count > 0)
                    {
                        DataTable dt = new DataTable();
                        DataRowCollection dtRow = null;
                        try
                        {
                            DataColumn colum = new DataColumn("QRCode");
                            colum.DataType = Type.GetType("System.Byte[]");
                            colum.AllowDBNull = true;
                            dt.Columns.Add(colum);

                            DataColumn colum23 = new DataColumn("QRCode2");
                            colum23.DataType = Type.GetType("System.Byte[]");
                            colum23.AllowDBNull = true;
                            dt.Columns.Add(colum23);

                            DataColumn colum2 = new DataColumn("HN");
                            colum2.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum2);

                            DataColumn colum3 = new DataColumn("PAPMI_RowId");
                            colum3.DataType = Type.GetType("System.Int32");
                            dt.Columns.Add(colum3);

                            DataColumn colum4 = new DataColumn("PAADM_RowID");
                            colum4.DataType = Type.GetType("System.Int32");
                            dt.Columns.Add(colum4);

                            DataColumn colum5 = new DataColumn("SSUSR_Name");
                            colum5.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum5);

                            DataColumn colum6 = new DataColumn("PAPMI_No");
                            colum6.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum6);

                            DataColumn colum7 = new DataColumn("PAPMI_DOB");
                            colum7.DataType = Type.GetType("System.DateTime");
                            dt.Columns.Add(colum7);

                            DataColumn colum8 = new DataColumn("CTPCP_Desc");
                            colum8.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum8);

                            DataColumn colum9 = new DataColumn("CTSEX_Desc");
                            colum9.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum9);

                            DataColumn colum10 = new DataColumn("PAPER_AgeYr");
                            colum10.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum10);

                            DataColumn colum11 = new DataColumn("PAPER_AgeMth");
                            colum11.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum11);

                            DataColumn colum12 = new DataColumn("CTLOC_Desc");
                            colum12.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum12);

                            DataColumn colum13 = new DataColumn("PAADM_AdmDate");
                            colum13.DataType = Type.GetType("System.DateTime");
                            dt.Columns.Add(colum13);

                            DataColumn colum14 = new DataColumn("PAADM_ADMNo");
                            colum14.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum14);

                            DataColumn colum15 = new DataColumn("PAPMI_Name3");
                            colum15.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum15);

                            DataColumn colum16 = new DataColumn("PAPMI_Name2");
                            colum16.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum16);

                            DataColumn colum17 = new DataColumn("PAPMI_Name");
                            colum17.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum17);

                            DataColumn colum18 = new DataColumn("CTPCP_Code");
                            colum18.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum18);

                            DataColumn colum19 = new DataColumn("CTLOC_Code");
                            colum19.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum19);

                            DataColumn colum20 = new DataColumn("Allergy");
                            colum20.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum20);

                            DataColumn colum21 = new DataColumn("Roomcode");
                            colum21.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum21);

                            DataColumn colum22 = new DataColumn("Wardcode");
                            colum22.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum22);
                            // CTPCP_DescEng
                            DataColumn colum24 = new DataColumn("CTPCP_DescEng");
                            colum24.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum24);


                            DataColumn colum25 = new DataColumn("PAPER_StNameLine1");
                            colum25.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum25);


                            DataColumn colum26 = new DataColumn("CTCIT_Desc");
                            colum26.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum26);


                            DataColumn colum27 = new DataColumn("CITAREA_Desc1");
                            colum27.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum27);


                            DataColumn colum28 = new DataColumn("PROV_Desc");
                            colum28.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum28);


                            DataColumn colum29 = new DataColumn("CTZIP_Code");
                            colum29.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum29);


                            DataColumn colum30 = new DataColumn("PAPER_TelH");
                            colum30.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum30);


                            DataColumn colum31 = new DataColumn("PAPER_Email");
                            colum31.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum31);


                            DataColumn colum32 = new DataColumn("PAPER_Fax");
                            colum32.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum32);


                            DataColumn colum33 = new DataColumn("QRCode3");
                            colum33.DataType = Type.GetType("System.Byte[]");
                            dt.Columns.Add(colum33);

                            DataColumn colum34 = new DataColumn("PAPMI_Name8");
                            colum34.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum34);

                            DataColumn colum35 = new DataColumn("CTMAR_Desc");
                            colum35.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum35);


                            DataColumn colum36 = new DataColumn("CTRLG_Desc");
                            colum36.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum36);

                            DataColumn colum37 = new DataColumn("CTOCC_Desc");
                            colum37.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum37);

                            DataColumn colum38 = new DataColumn("CTCOU_Desc");
                            colum38.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum38);

                            DataColumn colum39 = new DataColumn("CTPCP_SMCNo");
                            colum39.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum39);

                            DataColumn colum40 = new DataColumn("Doctype");
                            colum40.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum40);

                            DataColumn colum41 = new DataColumn("DocName");
                            colum41.DataType = Type.GetType("System.String");
                            dt.Columns.Add(colum41);

                            DataColumn colum42 = new DataColumn("QRCode4");
                            colum42.DataType = Type.GetType("System.Byte[]");
                            colum42.AllowDBNull = true;
                            dt.Columns.Add(colum42);

                            DataColumn colum43 = new DataColumn("QRCode5");
                            colum43.DataType = Type.GetType("System.Byte[]");
                            colum43.AllowDBNull = true;
                            dt.Columns.Add(colum43);

                            DataColumn colum44 = new DataColumn("QRCode6");
                            colum44.DataType = Type.GetType("System.Byte[]");
                            colum44.AllowDBNull = true;
                            dt.Columns.Add(colum44);

                            DataColumn colum45 = new DataColumn("QRCode7");
                            colum45.DataType = Type.GetType("System.Byte[]");
                            colum45.AllowDBNull = true;
                            dt.Columns.Add(colum45);

                            DataColumn colum46 = new DataColumn("QID");
                            colum46.DataType = Type.GetType("System.Int32");
                            dt.Columns.Add(colum46);
                        }
                        catch
                        {
                        }
                        //End  Create Datatable
                        #endregion
                        #region HN to QRCODE
                        string myText = "";
                        string myText2 = "";
                        string myText3 = "";
                        string myText4 = "";
                        string myText6 = "";
                        string myText5 = "";
                        string myText7 = "";
                        if (dtTrak.Rows[0]["PAPMI_No"] != null)
                        {
                            myText = dtTrak.Rows[0]["PAPMI_No"].ToString().Replace("-", "");
                            myText2 = dtTrak.Rows[0]["PAPMI_No"].ToString().Replace("-", "");
                            myText3 = dtTrak.Rows[0]["PAPMI_No"].ToString().Replace("-", "");
                            myText4 = dtTrak.Rows[0]["PAPMI_No"].ToString().Replace("-", "");
                            myText5 = dtTrak.Rows[0]["PAPMI_No"].ToString().Replace("-", "");
                            myText6 = dtTrak.Rows[0]["PAPMI_No"].ToString().Replace("-", "");
                            myText7 = dtTrak.Rows[0]["PAPMI_No"].ToString().Replace("-", "");
                        }
                        #endregion
                        #region Episode to QRCODE
                        if (dtTrak.Rows[0]["PAADM_ADMNo"] != null)
                        {
                            if (myText.Trim() != "")
                            {
                                myText = myText + " " + dtTrak.Rows[0]["PAADM_ADMNo"].ToString().Replace("-", "");
                            }
                            else
                            {
                                myText = dtTrak.Rows[0]["PAADM_ADMNo"].ToString().Replace("-", "");
                            }


                            if (myText2.Trim() != "")
                            {
                                myText2 = myText2 + " " + dtTrak.Rows[0]["PAADM_ADMNo"].ToString().Replace("-", "");
                            }
                            else
                            {
                                myText2 = dtTrak.Rows[0]["PAADM_ADMNo"].ToString().Replace("-", "");
                            }


                            if (myText3.Trim() != "")
                            {
                                myText3 = myText3 + " " + dtTrak.Rows[0]["PAADM_ADMNo"].ToString().Replace("-", "");
                            }
                            else
                            {
                                myText3 = dtTrak.Rows[0]["PAADM_ADMNo"].ToString().Replace("-", "");
                            }

                            if (myText4.Trim() != "")
                            {
                                myText4 = myText4 + " " + dtTrak.Rows[0]["PAADM_ADMNo"].ToString().Replace("-", "");
                            }
                            else
                            {
                                myText4 = dtTrak.Rows[0]["PAADM_ADMNo"].ToString().Replace("-", "");
                            }

                            if (myText5.Trim() != "")
                            {
                                myText5 = myText5 + " " + dtTrak.Rows[0]["PAADM_ADMNo"].ToString().Replace("-", "");
                            }
                            else
                            {
                                myText5 = dtTrak.Rows[0]["PAADM_ADMNo"].ToString().Replace("-", "");
                            }

                            if (myText6.Trim() != "")
                            {
                                myText6 = myText6 + " " + dtTrak.Rows[0]["PAADM_ADMNo"].ToString().Replace("-", "");
                            }
                            else
                            {
                                myText6 = dtTrak.Rows[0]["PAADM_ADMNo"].ToString().Replace("-", "");
                            }

                            if (myText7.Trim() != "")
                            {
                                myText7 = myText7 + " " + dtTrak.Rows[0]["PAADM_ADMNo"].ToString().Replace("-", "");
                            }
                            else
                            {
                                myText7 = dtTrak.Rows[0]["PAADM_ADMNo"].ToString().Replace("-", "");
                            }
                        }
                        #endregion

                        #region Page to QRCode
                        if (myText.Trim() != "")
                        {
                            myText = myText + " " + GetTypeDoc(ReportName) + "A1" + DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmm");
                        }
                        else
                        {
                            myText = GetTypeDoc(ReportName) + "A1" + DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmm");
                        }


                        if (myText2.Trim() != "")
                        {
                            myText2 = myText2 + " " + GetTypeDoc(ReportName) + "A2" + DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmm");
                        }
                        else
                        {
                            myText2 = GetTypeDoc(ReportName) + "A2" + DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmm");
                        }


                        if (myText3.Trim() != "")
                        {
                            myText3 = myText3 + " " + GetTypeDoc(ReportName) + "A3" + DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmm");
                        }
                        else
                        {
                            myText3 = GetTypeDoc(ReportName) + "A3" + DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmm");
                        }

                        if (myText4.Trim() != "")
                        {
                            myText4 = myText4 + " " + GetTypeDoc(ReportName) + "A4" + DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmm");
                        }
                        else
                        {
                            myText4 = GetTypeDoc(ReportName) + "A4" + DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmm");
                        }

                        if (myText5.Trim() != "")
                        {
                            myText5 = myText5 + " " + GetTypeDoc(ReportName) + "A5" + DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmm");
                        }
                        else
                        {
                            myText5 = GetTypeDoc(ReportName) + "A5" + DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmm");
                        }

                        if (myText6.Trim() != "")
                        {
                            myText6 = myText6 + " " + GetTypeDoc(ReportName) + "A6" + DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmm");
                        }
                        else
                        {
                            myText6 = GetTypeDoc(ReportName) + "A6" + DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmm");
                        }

                        if (myText7.Trim() != "")
                        {
                            myText7 = myText7 + " " + GetTypeDoc(ReportName) + "A7" + DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmm");
                        }
                        else
                        {
                            myText7 = GetTypeDoc(ReportName) + "A7" + DateTime.Now.ToString("yyyyMMdd") + DateTime.Now.ToString("HHmm");
                        }
                        #endregion

                        #region GetDoctor
                        //getDoctor---------------------------------------------------------
                        DataTable dtHaveCare = new DataTable();
                        string getCTPCP_Code = string.Empty;
                        #region Report for type KAD,JAB,JAA
                        if (DTConsen.Rows[0]["DocTtype"].ToString() == "KAD" || DTConsen.Rows[0]["DocTtype"].ToString() == "JAB" || (DTConsen.Rows[0]["DocTtype"].ToString() == "JAA"))
                        {
                            if (DTConsen.Rows[0]["StoreName"] != null) // Case Have Store and Parametore
                            {
                                if (DTConsen.Rows[0]["StoreName"].ToString() != "")
                                {
                                    if (DTConsen.Rows[0]["ParameterName1"] != null)
                                    {
                                        if (DTConsen.Rows[0]["ParameterName1"].ToString() != "")
                                        {
                                            getCTPCP_Code = "{CALL \"Custom_THSV_Report_ZEN_StoredProc\".\"" + DTConsen.Rows[0]["StoreName"].ToString() + "\".('" + Convert.ToInt32(EpisodeRowId) + "','" + QID + "')}";
                                        }
                                        else
                                        {
                                            getCTPCP_Code = "{CALL \"Custom_THSV_Report_ZEN_StoredProc\".\"" + DTConsen.Rows[0]["StoreName"].ToString() + "\".('" + Convert.ToInt32(EpisodeRowId) + "')}";
                                        }
                                    }
                                    else
                                    {
                                        getCTPCP_Code = "{CALL \"Custom_THSV_Report_ZEN_StoredProc\".\"" + DTConsen.Rows[0]["StoreName"].ToString() + "\".('" + Convert.ToInt32(EpisodeRowId) + "')}";
                                    }
                                    dtHaveCare.Clear();
                                    dtHaveCare = ODataManager.CallStoredProcedure(getCTPCP_Code);
                                    if (dtHaveCare.Rows.Count > 0)
                                    {
                                        if (dtHaveCare.Rows[0]["CTPCPCode"] != null)
                                        {
                                            if (myText.Trim() != "")
                                            {
                                                myText = myText + " " + dtHaveCare.Rows[0]["CTPCPCode"].ToString();
                                            }
                                            else
                                            {
                                                myText = dtHaveCare.Rows[0]["CTPCPCode"].ToString();
                                            }

                                            if (myText2.Trim() != "")
                                            {
                                                myText2 = myText2 + " " + dtHaveCare.Rows[0]["CTPCPCode"].ToString();
                                            }
                                            else
                                            {
                                                myText2 = dtHaveCare.Rows[0]["CTPCPCode"].ToString();
                                            }


                                            if (myText3.Trim() != "")
                                            {
                                                myText3 = myText3 + " " + dtHaveCare.Rows[0]["CTPCPCode"].ToString();
                                            }
                                            else
                                            {
                                                myText3 = dtHaveCare.Rows[0]["CTPCPCode"].ToString();
                                            }

                                            if (myText4.Trim() != "")
                                            {
                                                myText4 = myText4 + " " + dtHaveCare.Rows[0]["CTPCPCode"].ToString();
                                            }
                                            else
                                            {
                                                myText4 = dtHaveCare.Rows[0]["CTPCPCode"].ToString();
                                            }

                                            if (myText5.Trim() != "")
                                            {
                                                myText5 = myText5 + " " + dtHaveCare.Rows[0]["CTPCPCode"].ToString();
                                            }
                                            else
                                            {
                                                myText5 = dtHaveCare.Rows[0]["CTPCPCode"].ToString();
                                            }

                                            if (myText6.Trim() != "")
                                            {
                                                myText6 = myText6 + " " + dtHaveCare.Rows[0]["CTPCPCode"].ToString();
                                            }
                                            else
                                            {
                                                myText6 = dtHaveCare.Rows[0]["CTPCPCode"].ToString();
                                            }

                                            if (myText7.Trim() != "")
                                            {
                                                myText7 = myText7 + " " + dtHaveCare.Rows[0]["CTPCPCode"].ToString();
                                            }
                                            else
                                            {
                                                myText7 = dtHaveCare.Rows[0]["CTPCPCode"].ToString();
                                            }

                                        }
                                        else
                                        {

                                        }
                                    }
                                }
                            }
                            else
                            {

                            }

                            if (dtTrak.Rows[0]["CTLOC_Code"] != null)
                            {
                                if (myText.Trim() != "")
                                {
                                    myText = myText + ";" + dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                }
                                else
                                {
                                    myText = dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                }

                                if (myText2.Trim() != "")
                                {
                                    myText2 = myText2 + ";" + dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                }
                                else
                                {
                                    myText2 = dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                }


                                if (myText3.Trim() != "")
                                {
                                    myText3 = myText3 + ";" + dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                }
                                else
                                {
                                    myText3 = dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                }

                                if (myText4.Trim() != "")
                                {
                                    myText4 = myText4 + ";" + dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                }
                                else
                                {
                                    myText4 = dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                }

                                if (myText5.Trim() != "")
                                {
                                    myText5 = myText5 + ";" + dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                }
                                else
                                {
                                    myText5 = dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                }

                                if (myText6.Trim() != "")
                                {
                                    myText6 = myText6 + ";" + dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                }
                                else
                                {
                                    myText6 = dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                }

                                if (myText7.Trim() != "")
                                {
                                    myText7 = myText7 + ";" + dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                }
                                else
                                {
                                    myText7 = dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                }
                            }
                        }
                        #endregion
                        #region Report for AppId
                        else if (HasAppID) // getDoctorAppid
                        {
                            //13670
                            //AppID = "13860";

                            //dtApp = ODataManager.GetData("select LOC.CTLOC_CODE, CTLOC_Desc ,RBR.RES_CODE CTPCP_Code, RBR.RES_Desc CTPCP_Desc, RBR.RES_Desc CTPCP_DescEng from RB_Resource RBR   LEFT JOIN CT_LOC LOC ON (RES_CTLOC_DR=LOC.CTLOC_Rowid)  where res_rowid =" + AppID + "");
                            dtApp = ODataManager.GetData("select LOC.CTLOC_CODE, CTLOC_Desc, RBR.RES_CODE CTPCP_Code, CRP.CTPCP_Desc, CRP.CTPCP_StName CTPCP_DescEng from RB_Resource RBR LEFT JOIN CT_LOC LOC ON (RES_CTLOC_DR=LOC.CTLOC_Rowid) LEFT JOIN CT_CAREPROV CRP ON (CRP.CTPCP_Code = RBR.RES_CODE) where res_rowid =" + AppID + "");

                            if (dtApp.Rows[0]["CTPCP_Code"] != null)
                            {
                                if (myText.Trim() != "")
                                {
                                    myText = myText + " " + dtApp.Rows[0]["CTPCP_Code"].ToString();
                                }
                                else
                                {
                                    myText = dtApp.Rows[0]["CTPCP_Code"].ToString();
                                }

                                if (myText2.Trim() != "")
                                {
                                    myText2 = myText2 + " " + dtApp.Rows[0]["CTPCP_Code"].ToString();
                                }
                                else
                                {
                                    myText2 = dtApp.Rows[0]["CTPCP_Code"].ToString();
                                }


                                if (myText3.Trim() != "")
                                {
                                    myText3 = myText3 + " " + dtApp.Rows[0]["CTPCP_Code"].ToString();
                                }
                                else
                                {
                                    myText3 = dtApp.Rows[0]["CTPCP_Code"].ToString();
                                }

                                if (myText4.Trim() != "")
                                {
                                    myText4 = myText4 + " " + dtApp.Rows[0]["CTPCP_Code"].ToString();
                                }
                                else
                                {
                                    myText4 = dtApp.Rows[0]["CTPCP_Code"].ToString();
                                }

                                if (myText5.Trim() != "")
                                {
                                    myText5 = myText5 + " " + dtApp.Rows[0]["CTPCP_Code"].ToString();
                                }
                                else
                                {
                                    myText5 = dtApp.Rows[0]["CTPCP_Code"].ToString();
                                }

                                if (myText6.Trim() != "")
                                {
                                    myText6 = myText6 + " " + dtApp.Rows[0]["CTPCP_Code"].ToString();
                                }
                                else
                                {
                                    myText6 = dtApp.Rows[0]["CTPCP_Code"].ToString();
                                }

                                if (myText7.Trim() != "")
                                {
                                    myText7 = myText7 + " " + dtApp.Rows[0]["CTPCP_Code"].ToString();
                                }
                                else
                                {
                                    myText7 = dtApp.Rows[0]["CTPCP_Code"].ToString();
                                }
                            }
                            if (dtApp.Rows[0]["CTLOC_Code"] != null)
                            {
                                if (myText.Trim() != "")
                                {
                                    myText = myText + ";" + dtApp.Rows[0]["CTLOC_Code"].ToString();
                                }
                                else
                                {
                                    myText = dtApp.Rows[0]["CTLOC_Code"].ToString();
                                }

                                if (myText2.Trim() != "")
                                {
                                    myText2 = myText2 + ";" + dtApp.Rows[0]["CTLOC_Code"].ToString();
                                }
                                else
                                {
                                    myText2 = dtApp.Rows[0]["CTLOC_Code"].ToString();
                                }


                                if (myText3.Trim() != "")
                                {
                                    myText3 = myText3 + ";" + dtApp.Rows[0]["CTLOC_Code"].ToString();
                                }
                                else
                                {
                                    myText3 = dtApp.Rows[0]["CTLOC_Code"].ToString();
                                }

                                if (myText4.Trim() != "")
                                {
                                    myText4 = myText4 + ";" + dtApp.Rows[0]["CTLOC_Code"].ToString();
                                }
                                else
                                {
                                    myText4 = dtApp.Rows[0]["CTLOC_Code"].ToString();
                                }

                                if (myText5.Trim() != "")
                                {
                                    myText5 = myText5 + ";" + dtApp.Rows[0]["CTLOC_Code"].ToString();
                                }
                                else
                                {
                                    myText5 = dtApp.Rows[0]["CTLOC_Code"].ToString();
                                }

                                if (myText6.Trim() != "")
                                {
                                    myText6 = myText6 + ";" + dtApp.Rows[0]["CTLOC_Code"].ToString();
                                }
                                else
                                {
                                    myText6 = dtApp.Rows[0]["CTLOC_Code"].ToString();
                                }

                                if (myText7.Trim() != "")
                                {
                                    myText7 = myText7 + ";" + dtApp.Rows[0]["CTLOC_Code"].ToString();
                                }
                                else
                                {
                                    myText7 = dtApp.Rows[0]["CTLOC_Code"].ToString();
                                }
                            }

                        }//end getDoctor from appId
                        #endregion
                        #region getDoctorByCprowid
                        else if (HasCPRowID) // getDoctorCprowid
                        {
                            dtcprowid = ODataManager.GetData("select CTPCP_Code,CTPCP_Desc,CTPCP_StName from ct_careprov where ctpcp_rowid1 = " + CPRowID + " ");

                            if (dtcprowid.Rows[0]["CTPCP_Code"] != null)
                            {
                                if (myText.Trim() != "")
                                {
                                    myText = myText + " " + dtcprowid.Rows[0]["CTPCP_Code"].ToString();
                                }
                                else
                                {
                                    myText = dtcprowid.Rows[0]["CTPCP_Code"].ToString();
                                }

                                if (myText2.Trim() != "")
                                {
                                    myText2 = myText2 + " " + dtcprowid.Rows[0]["CTPCP_Code"].ToString();
                                }
                                else
                                {
                                    myText2 = dtcprowid.Rows[0]["CTPCP_Code"].ToString();
                                }


                                if (myText3.Trim() != "")
                                {
                                    myText3 = myText3 + " " + dtcprowid.Rows[0]["CTPCP_Code"].ToString();
                                }
                                else
                                {
                                    myText3 = dtcprowid.Rows[0]["CTPCP_Code"].ToString();
                                }

                                if (myText4.Trim() != "")
                                {
                                    myText4 = myText4 + " " + dtcprowid.Rows[0]["CTPCP_Code"].ToString();
                                }
                                else
                                {
                                    myText4 = dtcprowid.Rows[0]["CTPCP_Code"].ToString();
                                }

                                if (myText5.Trim() != "")
                                {
                                    myText5 = myText5 + " " + dtcprowid.Rows[0]["CTPCP_Code"].ToString();
                                }
                                else
                                {
                                    myText5 = dtcprowid.Rows[0]["CTPCP_Code"].ToString();
                                }

                                if (myText6.Trim() != "")
                                {
                                    myText6 = myText6 + " " + dtcprowid.Rows[0]["CTPCP_Code"].ToString();
                                }
                                else
                                {
                                    myText6 = dtcprowid.Rows[0]["CTPCP_Code"].ToString();
                                }

                                if (myText7.Trim() != "")
                                {
                                    myText7 = myText7 + " " + dtcprowid.Rows[0]["CTPCP_Code"].ToString();
                                }
                                else
                                {
                                    myText7 = dtcprowid.Rows[0]["CTPCP_Code"].ToString();
                                }

                            }
                            //get Loc
                            if (dtTrak.Rows[0]["CTLOC_Code"] != null)
                            {
                                if (myText.Trim() != "")
                                {
                                    myText = myText + ";" + dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                }
                                else
                                {
                                    myText = dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                }

                                if (myText2.Trim() != "")
                                {
                                    myText2 = myText2 + ";" + dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                }
                                else
                                {
                                    myText2 = dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                }


                                if (myText3.Trim() != "")
                                {
                                    myText3 = myText3 + ";" + dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                }
                                else
                                {
                                    myText3 = dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                }

                                if (myText4.Trim() != "")
                                {
                                    myText4 = myText4 + ";" + dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                }
                                else
                                {
                                    myText4 = dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                }

                                if (myText5.Trim() != "")
                                {
                                    myText5 = myText5 + ";" + dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                }
                                else
                                {
                                    myText5 = dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                }

                                if (myText6.Trim() != "")
                                {
                                    myText6 = myText6 + ";" + dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                }
                                else
                                {
                                    myText6 = dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                }

                                if (myText7.Trim() != "")
                                {
                                    myText7 = myText7 + ";" + dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                }
                                else
                                {
                                    myText7 = dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                }
                            }

                        }//end getDoctor from cprowid
                        #endregion
                        //#region Other Report
                        else
                        {
                            //DTConsen.Rows[0]["DocTtype"].ToString() == "KAD"
                            #region Episode = I and TYPE = IGA
                            if (dtTrak.Rows[0]["PAADM_ADMNO"].ToString().Substring(0, 1) == "I" && DTConsen.Rows[0]["DocTtype"].ToString() == "IGA")
                            {
                                myText = myText + " " + "00000000";
                                myText2 = myText2 + " " + "00000000";
                                myText3 = myText3 + " " + "00000000";
                                myText4 = myText4 + " " + "00000000";
                                myText5 = myText5 + " " + "00000000";
                                myText6 = myText6 + " " + "00000000";
                                myText7 = myText7 + " " + "00000000";
                            }
                            #endregion

                            else
                            {
                                #region Carepro to QRCode
                                if (dtTrak.Rows[0]["CTPCP_Code"] != null)
                                {
                                    if (myText.Trim() != "")
                                    {
                                        myText = myText + " " + dtTrak.Rows[0]["CTPCP_Code"].ToString();
                                    }
                                    else
                                    {
                                        myText = dtTrak.Rows[0]["CTPCP_Code"].ToString();
                                    }

                                    if (myText2.Trim() != "")
                                    {
                                        myText2 = myText2 + " " + dtTrak.Rows[0]["CTPCP_Code"].ToString();
                                    }
                                    else
                                    {
                                        myText2 = dtTrak.Rows[0]["CTPCP_Code"].ToString();
                                    }


                                    if (myText3.Trim() != "")
                                    {
                                        myText3 = myText3 + " " + dtTrak.Rows[0]["CTPCP_Code"].ToString();
                                    }
                                    else
                                    {
                                        myText3 = dtTrak.Rows[0]["CTPCP_Code"].ToString();
                                    }

                                    if (myText4.Trim() != "")
                                    {
                                        myText4 = myText4 + " " + dtTrak.Rows[0]["CTPCP_Code"].ToString();
                                    }
                                    else
                                    {
                                        myText4 = dtTrak.Rows[0]["CTPCP_Code"].ToString();
                                    }

                                    if (myText5.Trim() != "")
                                    {
                                        myText5 = myText5 + " " + dtTrak.Rows[0]["CTPCP_Code"].ToString();
                                    }
                                    else
                                    {
                                        myText5 = dtTrak.Rows[0]["CTPCP_Code"].ToString();
                                    }

                                    if (myText6.Trim() != "")
                                    {
                                        myText6 = myText6 + " " + dtTrak.Rows[0]["CTPCP_Code"].ToString();
                                    }
                                    else
                                    {
                                        myText6 = dtTrak.Rows[0]["CTPCP_Code"].ToString();
                                    }

                                    if (myText7.Trim() != "")
                                    {
                                        myText7 = myText7 + " " + dtTrak.Rows[0]["CTPCP_Code"].ToString();
                                    }
                                    else
                                    {
                                        myText7 = dtTrak.Rows[0]["CTPCP_Code"].ToString();
                                    }
                                }
                                #endregion
                            }
                            #region get location Episode I and TYPE IGA
                            //get Location for else
                            if (dtTrak.Rows[0]["PAADM_ADMNO"].ToString().Substring(0, 1) == "I" && DTConsen.Rows[0]["DocTtype"].ToString() == "IGA")
                            {
                                if (dtTrak.Rows[0]["WARD_Code"] != null)
                                {
                                    if (myText.Trim() != "")
                                    {
                                        myText = myText + ";" + dtTrak.Rows[0]["WARD_Code"].ToString();
                                    }
                                    else
                                    {
                                        myText = dtTrak.Rows[0]["WARD_Code"].ToString();
                                    }

                                    if (myText2.Trim() != "")
                                    {
                                        myText2 = myText2 + ";" + dtTrak.Rows[0]["WARD_Code"].ToString();
                                    }
                                    else
                                    {
                                        myText2 = dtTrak.Rows[0]["WARD_Code"].ToString();
                                    }


                                    if (myText3.Trim() != "")
                                    {
                                        myText3 = myText3 + ";" + dtTrak.Rows[0]["WARD_Code"].ToString();
                                    }
                                    else
                                    {
                                        myText3 = dtTrak.Rows[0]["WARD_Code"].ToString();
                                    }

                                    if (myText4.Trim() != "")
                                    {
                                        myText4 = myText4 + ";" + dtTrak.Rows[0]["WARD_Code"].ToString();
                                    }
                                    else
                                    {
                                        myText4 = dtTrak.Rows[0]["WARD_Code"].ToString();
                                    }

                                    if (myText5.Trim() != "")
                                    {
                                        myText5 = myText5 + ";" + dtTrak.Rows[0]["WARD_Code"].ToString();
                                    }
                                    else
                                    {
                                        myText5 = dtTrak.Rows[0]["WARD_Code"].ToString();
                                    }

                                    if (myText6.Trim() != "")
                                    {
                                        myText6 = myText6 + ";" + dtTrak.Rows[0]["WARD_Code"].ToString();
                                    }
                                    else
                                    {
                                        myText6 = dtTrak.Rows[0]["WARD_Code"].ToString();
                                    }

                                    if (myText7.Trim() != "")
                                    {
                                        myText7 = myText7 + ";" + dtTrak.Rows[0]["WARD_Code"].ToString();
                                    }
                                    else
                                    {
                                        myText7 = dtTrak.Rows[0]["WARD_Code"].ToString();
                                    }
                                }
                            }
                            #endregion

                            #region get location
                            else
                            {
                                if (dtTrak.Rows[0]["CTLOC_Code"] != null)
                                {
                                    if (myText.Trim() != "")
                                    {
                                        myText = myText + ";" + dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                    }
                                    else
                                    {
                                        myText = dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                    }

                                    if (myText2.Trim() != "")
                                    {
                                        myText2 = myText2 + ";" + dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                    }
                                    else
                                    {
                                        myText2 = dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                    }


                                    if (myText3.Trim() != "")
                                    {
                                        myText3 = myText3 + ";" + dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                    }
                                    else
                                    {
                                        myText3 = dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                    }

                                    if (myText4.Trim() != "")
                                    {
                                        myText4 = myText4 + ";" + dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                    }
                                    else
                                    {
                                        myText4 = dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                    }

                                    if (myText5.Trim() != "")
                                    {
                                        myText5 = myText5 + ";" + dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                    }
                                    else
                                    {
                                        myText5 = dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                    }

                                    if (myText6.Trim() != "")
                                    {
                                        myText6 = myText6 + ";" + dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                    }
                                    else
                                    {
                                        myText6 = dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                    }

                                    if (myText7.Trim() != "")
                                    {
                                        myText7 = myText7 + ";" + dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                    }
                                    else
                                    {
                                        myText7 = dtTrak.Rows[0]["CTLOC_Code"].ToString();
                                    }
                                }
                            }
                            #endregion
                        }
                        // End Doctor---------------------------------------------------------
                        #endregion

                        if ((myText.Length > 0) && (ReportName.Length > 0))
                        {
                            #region Set Value to DataTable
                            // bm.Save(Server.MapPath("~/bin/QRCode.png"));//Test Save Data
                            DataRow R = dt.NewRow();
                            if (dtTrak.Rows[0]["PAPMI_No"] != null) { R["HN"] = dtTrak.Rows[0]["PAPMI_No"].ToString(); }
                            if (dtTrak.Rows[0]["PAPMI_RowId"] != null) { R["PAPMI_RowId"] = dtTrak.Rows[0]["PAPMI_RowId"].ToString(); }
                            if (dtTrak.Rows[0]["PAADM_RowID"] != null) { R["PAADM_RowID"] = dtTrak.Rows[0]["PAADM_RowID"].ToString(); }
                            if (dtTrak.Rows[0]["SSUSR_Name"] != null) { R["SSUSR_Name"] = dtTrak.Rows[0]["SSUSR_Name"].ToString(); }
                            if (dtTrak.Rows[0]["PAPMI_No"] != null) { R["PAPMI_No"] = dtTrak.Rows[0]["PAPMI_No"].ToString(); }
                            if (dtTrak.Rows[0]["PAPMI_DOB"] != null) { R["PAPMI_DOB"] = dtTrak.Rows[0]["PAPMI_DOB"].ToString(); }
                            //if (dtTrak.Rows[0]["CTPCP_Desc"] != null) { R["CTPCP_Desc"] = dtTrak.Rows[0]["CTPCP_Desc"].ToString(); }
                            if (HasAppID) { R["CTPCP_Desc"] = dtApp.Rows[0]["CTPCP_Desc"].ToString(); } else if (HasCPRowID) { R["CTPCP_Desc"] = dtcprowid.Rows[0]["CTPCP_Desc"].ToString(); } else { R["CTPCP_Desc"] = dtTrak.Rows[0]["CTPCP_Desc"].ToString(); }
                            if (dtTrak.Rows[0]["CTSEX_Desc"] != null) { R["CTSEX_Desc"] = dtTrak.Rows[0]["CTSEX_Desc"].ToString(); }
                            if (dtTrak.Rows[0]["PAPER_AgeYr"] != null) { R["PAPER_AgeYr"] = dtTrak.Rows[0]["PAPER_AgeYr"].ToString(); }
                            if (dtTrak.Rows[0]["PAPER_AgeMth"] != null) { R["PAPER_AgeMth"] = dtTrak.Rows[0]["PAPER_AgeMth"].ToString(); }
                            if (dtTrak.Rows[0]["CTLOC_Desc"] != null) { R["CTLOC_Desc"] = dtTrak.Rows[0]["CTLOC_Desc"].ToString(); }
                            if (dtTrak.Rows[0]["PAADM_AdmDate"] != null) { R["PAADM_AdmDate"] = dtTrak.Rows[0]["PAADM_AdmDate"].ToString(); }
                            if (dtTrak.Rows[0]["PAADM_ADMNo"] != null) { R["PAADM_ADMNo"] = dtTrak.Rows[0]["PAADM_ADMNo"].ToString(); }
                            if (dtTrak.Rows[0]["PAPMI_Name3"] != null) { R["PAPMI_Name3"] = dtTrak.Rows[0]["PAPMI_Name3"].ToString(); }
                            if (dtTrak.Rows[0]["PAPMI_Name2"] != null) { R["PAPMI_Name2"] = dtTrak.Rows[0]["PAPMI_Name2"].ToString(); }
                            if (dtTrak.Rows[0]["PAPMI_Name"] != null) { R["PAPMI_Name"] = dtTrak.Rows[0]["PAPMI_Name"].ToString(); }
                            if (dtTrak.Rows[0]["CTPCP_Code"] != null) { R["CTPCP_Code"] = dtTrak.Rows[0]["CTPCP_Code"].ToString(); }
                            if (dtTrak.Rows[0]["CTLOC_Code"] != null) { R["CTLOC_Code"] = dtTrak.Rows[0]["CTLOC_Code"].ToString(); }
                            if (GetAlg(Convert.ToInt32(dtTrak.Rows[0]["PAPMI_RowId"])) != "") { R["Allergy"] = GetAlg(Convert.ToInt32(dtTrak.Rows[0]["PAPMI_RowId"])); }
                            if (dtTrak.Rows[0]["ROOM_Desc"] != null) { R["Roomcode"] = dtTrak.Rows[0]["ROOM_Desc"].ToString(); }
                            if (dtTrak.Rows[0]["WARD_Desc"] != null) { R["Wardcode"] = dtTrak.Rows[0]["WARD_Desc"].ToString(); }
                            //if (dtTrak.Rows[0]["CTPCP_StName"] != null && !HasAppID) { R["CTPCP_DescEng"] = dtTrak.Rows[0]["CTPCP_StName"].ToString(); } else { R["CTPCP_DescEng"] = dtApp.Rows[0]["CTPCP_DescEng"].ToString(); }
                            if (HasAppID) { R["CTPCP_DescEng"] = dtApp.Rows[0]["CTPCP_DescEng"].ToString(); } else if (HasCPRowID) { R["CTPCP_DescEng"] = dtcprowid.Rows[0]["CTPCP_StName"].ToString(); } else { R["CTPCP_DescEng"] = dtTrak.Rows[0]["CTPCP_StName"].ToString(); }

                            if (dtTrak.Rows[0]["PAPER_StNameLine1"] != null) { R["PAPER_StNameLine1"] = dtTrak.Rows[0]["PAPER_StNameLine1"].ToString(); }
                            if (dtTrak.Rows[0]["CTCIT_Desc"] != null) { R["CTCIT_Desc"] = dtTrak.Rows[0]["CTCIT_Desc"].ToString(); } //เขต
                            if (dtTrak.Rows[0]["CITAREA_Desc1"] != null) { R["CITAREA_Desc1"] = dtTrak.Rows[0]["CITAREA_Desc1"].ToString(); } //แขวง
                            if (dtTrak.Rows[0]["PROV_Desc"] != null) { R["PROV_Desc"] = dtTrak.Rows[0]["PROV_Desc"].ToString(); }
                            if (dtTrak.Rows[0]["CTZIP_Code"] != null) { R["CTZIP_Code"] = dtTrak.Rows[0]["CTZIP_Code"].ToString(); }
                            if (dtTrak.Rows[0]["PAPER_TelH"] != null) { R["PAPER_TelH"] = dtTrak.Rows[0]["PAPER_TelH"].ToString(); }
                            if (dtTrak.Rows[0]["PAPER_Email"] != null) { R["PAPER_Email"] = dtTrak.Rows[0]["PAPER_Email"].ToString(); }
                            if (dtTrak.Rows[0]["PAPER_Fax"] != null) { R["PAPER_Fax"] = dtTrak.Rows[0]["PAPER_Fax"].ToString(); }
                            if (dtTrak.Rows[0]["PAPMI_Name8"] != null) { R["PAPMI_Name8"] = dtTrak.Rows[0]["PAPMI_Name8"].ToString(); }
                            if (dtTrak.Rows[0]["CTMAR_Desc"] != null) { R["CTMAR_Desc"] = dtTrak.Rows[0]["CTMAR_Desc"].ToString(); }
                            if (dtTrak.Rows[0]["CTRLG_Desc"] != null) { R["CTRLG_Desc"] = dtTrak.Rows[0]["CTRLG_Desc"].ToString(); }
                            if (dtTrak.Rows[0]["CTOCC_Desc"] != null) { R["CTOCC_Desc"] = dtTrak.Rows[0]["CTOCC_Desc"].ToString(); }
                            if (dtTrak.Rows[0]["CTCOU_Desc"] != null) { R["CTCOU_Desc"] = dtTrak.Rows[0]["CTCOU_Desc"].ToString(); }
                            if (dtTrak.Rows[0]["CTPCP_SMCNo"] != null) { R["CTPCP_SMCNo"] = dtTrak.Rows[0]["CTPCP_SMCNo"].ToString(); }
                            if (QID != 0) { R["QID"] = QID; }

                            R["Doctype"] = GetTypeDoc(ReportName);
                            R["DocName"] = DocNameT;

                            byte[] bytes;
                            try
                            {


                                qe.QRCodeVersion = 4;


                                //Set Data to QRCODE
                                System.Drawing.Bitmap bm = qe.Encode(myText);


                                using (var memoryStream = new MemoryStream())
                                {
                                    bm.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

                                    bytes = memoryStream.ToArray();
                                }

                                System.Drawing.Bitmap bm2 = qe.Encode(myText2);

                                byte[] bytes2;
                                using (var memoryStream = new MemoryStream())
                                {
                                    bm2.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

                                    bytes2 = memoryStream.ToArray();
                                }



                                System.Drawing.Bitmap bm3 = qe.Encode(myText3);

                                byte[] bytes3;
                                using (var memoryStream = new MemoryStream())
                                {
                                    bm3.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

                                    bytes3 = memoryStream.ToArray();
                                }

                                System.Drawing.Bitmap bm4 = qe.Encode(myText4);

                                byte[] bytes4;
                                using (var memoryStream = new MemoryStream())
                                {
                                    bm4.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

                                    bytes4 = memoryStream.ToArray();
                                }

                                System.Drawing.Bitmap bm5 = qe.Encode(myText5);

                                byte[] bytes5;
                                using (var memoryStream = new MemoryStream())
                                {
                                    bm5.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

                                    bytes5 = memoryStream.ToArray();
                                }

                                System.Drawing.Bitmap bm6 = qe.Encode(myText6);

                                byte[] bytes6;
                                using (var memoryStream = new MemoryStream())
                                {
                                    bm6.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

                                    bytes6 = memoryStream.ToArray();
                                }

                                System.Drawing.Bitmap bm7 = qe.Encode(myText7);

                                byte[] bytes7;
                                using (var memoryStream = new MemoryStream())
                                {
                                    bm7.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

                                    bytes7 = memoryStream.ToArray();
                                }

                                R["QRCode3"] = bytes3;
                                R["QRCode2"] = bytes2;
                                R["QRCode"] = bytes;
                                R["QRCode4"] = bytes4;
                                R["QRCode5"] = bytes5;
                                R["QRCode6"] = bytes6;
                                R["QRCode7"] = bytes7;
                            }
                            catch (System.Exception ex)
                            {
                                Exception realerror = ex;
                                while (realerror.InnerException != null)
                                    realerror = realerror.InnerException;

                                Response.Write(realerror);
                                return;
                            }
                            dt.Rows.Add(R);
                            #endregion 

                            if (dt.Rows.Count > 0)
                            {
                                try
                                {
                                    #region get path from sql
                                    // Find Path From Table sql21----------------------------------------------------
                                    string Path = "";
                                    string PathName = "";
                                    DataTable DTReportName = new DataTable();
                                    if (ReportName != "")
                                    {
                                        try
                                        {
                                            DTReportName = ODataManager.GetDataSQL("select  ReportPath  from  ConsentFromRef  where  DocCallName ='" + ReportName + "'");
                                            if (DTReportName.Rows.Count > 0)
                                            {
                                                if (DTReportName.Rows[0]["ReportPath"] != null)
                                                {
                                                    PathName = DTReportName.Rows[0]["ReportPath"].ToString();
                                                }
                                            }
                                        }
                                        catch { }
                                        if (PathName != "")
                                        {
                                            Path = Server.MapPath(PathName);
                                        }
                                    }
                                    // End   Find Path From Table sql21----------------------------------------------------
                                    #endregion
                                    #region Load path
                                    // Load path -------------------
                                    cryRpt = new ReportDocument();
                                    try
                                    {
                                        cryRpt.Load(Path);
                                    }
                                    catch (System.Exception ex) { }
                                    // End load path------------------------
                                    #endregion

                                    #region get data from store
                                    DataTable dtHaveEN = new DataTable();
                                    //  Case Not  Consent  Check Parameter and StoreProcedure ----------------------
                                    DTReportName.Clear();
                                    if (ReportName != "" && ReportName != "AdmissionNote")
                                    {
                                        try
                                        {
                                            string sqlCheckEN = "";
                                            DTReportName = ODataManager.GetDataSQL("select  StoreName,ParameterName,ParameterName1,DocCallName  from  ConsentFromRef  where  DocCallName ='" + ReportName + "'");
                                            if (DTReportName.Rows.Count > 0)
                                            {
                                                if (DTReportName.Rows[0]["StoreName"] != null) // Case Have Store and Parametore
                                                {
                                                    if (DTReportName.Rows[0]["StoreName"].ToString() != "")
                                                    {
                                                        if (DTReportName.Rows[0]["ParameterName1"] != null)
                                                        {
                                                            if (DTReportName.Rows[0]["ParameterName1"].ToString() != "")
                                                            {
                                                                //CpRowid, [qid, rqid]
                                                                if (DTReportName.Rows[0]["ParameterName1"].ToString().Contains("QID"))
                                                                {
                                                                    sqlCheckEN = "{CALL \"Custom_THSV_Report_ZEN_StoredProc\".\"" + DTReportName.Rows[0]["StoreName"].ToString() + "\".('" + Convert.ToInt32(EpisodeRowId) + "','" + QID + "')}";
                                                                    //sqlCheckEN = "{CALL \"Custom_THSV_Report_ZEN_StoredProc\".\"SVNHReportMedCerOnTrakCare_getData \".('" + Convert.ToInt32(EpisodeRowId) + "','" +Convert.ToInt32(QID) + "')}";
                                                                    // sqlCheckEN = "{CALL \"Custom_THSV_Report_ZEN_StoredProc\".\"" + DTReportName.Rows[0]["StoreName"].ToString() + "\".('" + Convert.ToInt32(EpisodeRowId) + "')}";
                                                                }
                                                                else if (DTReportName.Rows[0]["ParameterName1"].ToString().Contains("CpRowid"))
                                                                {
                                                                    sqlCheckEN = "{CALL \"Custom_THSV_Report_ZEN_StoredProc\".\"" + DTReportName.Rows[0]["StoreName"].ToString() + "\".('" + Convert.ToInt32(EpisodeRowId) + "','" + CPRowID + "')}";
                                                                }
                                                                else
                                                                {
                                                                    sqlCheckEN = "{CALL \"Custom_THSV_Report_ZEN_StoredProc\".\"" + DTReportName.Rows[0]["StoreName"].ToString() + "\".('" + Convert.ToInt32(EpisodeRowId) + "','" + QID + "')}";
                                                                }
                                                                //sqlCheckEN = "{CALL \"Custom_THSV_Report_ZEN_StoredProc\".\"" + DTReportName.Rows[0]["StoreName"].ToString() + "\".('" + Convert.ToInt32(EpisodeRowId) + "','" + QID + "')}";
                                                            }
                                                            else
                                                            {
                                                                sqlCheckEN = "{CALL \"Custom_THSV_Report_ZEN_StoredProc\".\"" + DTReportName.Rows[0]["StoreName"].ToString() + "\".('" + Convert.ToInt32(EpisodeRowId) + "')}";
                                                            }

                                                        }
                                                        else
                                                        {
                                                            sqlCheckEN = "{CALL \"Custom_THSV_Report_ZEN_StoredProc\".\"" + DTReportName.Rows[0]["StoreName"].ToString() + "\".('" + Convert.ToInt32(EpisodeRowId) + "')}";
                                                        }

                                                        dtHaveEN.Clear();
                                                        dtHaveEN = ODataManager.CallStoredProcedure(sqlCheckEN);
                                                        //dtHaveEN1 = ODataManager.CallStoredProcedure1(sqlCheckEN);
                                                        //if (dtHaveEN.Rows.Count > 0)
                                                        //{
                                                        //    if (dtHaveEN.Rows[0]["TextQRCode"].ToString().Trim() == "")
                                                        //    {
                                                        //        dtHaveEN.Rows[0]["TextQRCode"] = myText;
                                                        //    }

                                                        //}

                                                        if (DTReportName.Rows[0]["ParameterName"] != null)
                                                        {
                                                            if (DTReportName.Rows[0]["ParameterName"].ToString() != "")
                                                            {
                                                                cryRpt.SetParameterValue(DTReportName.Rows[0]["ParameterName"].ToString().Trim(), EpisodeRowId);
                                                            }
                                                        }

                                                        if (DTReportName.Rows[0]["ParameterName1"] != null)
                                                        {
                                                            if (DTReportName.Rows[0]["ParameterName1"].ToString() != "")
                                                            {
                                                                if (QID != 0)
                                                                {
                                                                    cryRpt.SetParameterValue(DTReportName.Rows[0]["ParameterName1"].ToString().Trim(), QID);
                                                                }
                                                                else
                                                                {
                                                                    cryRpt.SetParameterValue(DTReportName.Rows[0]["ParameterName1"].ToString().Trim(), CPRowID);
                                                                }
                                                                //cryRpt.SetParameterValue(DTReportName.Rows[0]["ParameterName1"].ToString().Trim(), QID);
                                                                cryRpt.Subreports["QRCode.rpt"].SetDataSource(dt);
                                                                cryRpt.SetDataSource(dtHaveEN);
                                                            }
                                                            else
                                                            {
                                                                cryRpt.SetDataSource(dtHaveEN);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            cryRpt.SetDataSource(dtHaveEN);
                                                        }


                                                    }
                                                    else
                                                    {
                                                        cryRpt.SetDataSource(dt);

                                                    }
                                                }
                                                else // Case Consent
                                                {
                                                    cryRpt.SetDataSource(dt);

                                                }
                                            }
                                        }
                                        catch (System.Exception ex) { }
                                    }
                                    #endregion

                                    try
                                    {
                                        //cryRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, HttpContext.Current.Response, false, "QRCodeReport");
                                        cryRpt.ExportToHttpResponse(ExportFormatType.PortableDocFormat, HttpContext.Current.Response, false, "QRCodeReport");
                                        cryRpt.Dispose();
                                    }

                                    catch (System.Exception ex)
                                    //     catch (System.Threading.ThreadAbortException ex)
                                    {
                                        // cryRpt.Dispose();
                                        Thread.ResetAbort();
                                    }
                                }
                                catch (ThreadAbortException)
                                {
                                    // String aa = ex.Message;
                                    //MessageBox.show();
                                }
                            }
                        }
                    } // End Check dttrak
                }
                catch (System.Threading.ThreadAbortException ex)
                {
                    // string aa = "";
                    String aa = ex.Message;
                }
            } 
            #endregion //End if CheckEPI
         
            
            #region AdmissionNoteReport
            else if (ReportName == "AdmissionNote" || docType == "admnte") //getAdmission
            {
                if (QID != 0)
                {
                    GetReport(ReportName, QID, "18235");
                }
                //report.ExportToHttpResponse(ExportFormatType.PortableDocFormat, HttpContext.Current.Response, false, ReportName);
            }
            #endregion
            else if (docType == "QN")
            {
                if (QID != 0)
                {
                    GetReport(ReportName, QID, "");
                }
            }

            #region HNFlaxcliam
            else
            {
                if (Hn.Length > 0)
                {
                    if (Room == null) { Room = ""; }
                    GetReportFlaxCliam(ReportName, Hn, Room);
                }
            }

# endregion
            //#region AdmissionNoteReport
            //else if (ReportName == "DoctorInsurance") //getDoctorInsurance
            //{
            //    GetReport(ReportName, QID, EpisodeRowId);
            //}
            //#endregion
        }
    }
}
