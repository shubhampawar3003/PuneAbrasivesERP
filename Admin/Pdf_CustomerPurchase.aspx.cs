using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Admin_Pdf_CustomerPurchase : System.Web.UI.Page
{
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
    CommonCls objcls = new CommonCls();
    string ID = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["UserCode"] == null)
        {
            Response.Redirect("../Login.aspx");
        }
        else
        {
            if (Request.QueryString["Pono"] != null)
            {
                ID = objcls.Decrypt(Request.QueryString["Pono"].ToString());
                // ID = Request.QueryString["Pono"].ToString();
                PDF(ID);
            }
        }
    }

    private void PDF(string PO_NO)
    {
        string OrderUser = string.Empty;
        try
        {
            DataTable Dt = new DataTable();
            SqlDataAdapter Da = new SqlDataAdapter("SELECT * FROM [tbl_CustomerPurchaseOrderHdr] AS CPO INNER JOIN tbl_CompanyMaster AS CM ON CM.Companyname=CPO.CustomerName WHERE Pono='" + PO_NO + "'  ", con);
            Da.Fill(Dt);

            StringWriter sw = new StringWriter();
            StringReader sr = new StringReader(sw.ToString());

            Document doc = new Document(PageSize.A4, 10f, 10f, 55f, 0f);

            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(Server.MapPath("~/PDF_Files/") + "OA3.pdf", FileMode.Create));

            doc.Open();
            string UserName = Dt.Rows[0]["UserName"] != DBNull.Value ? Dt.Rows[0]["UserName"].ToString() : string.Empty;
            if (UserName != null && UserName != "")
            {
                DataTable dt = new DataTable();
                SqlDataAdapter Das = new SqlDataAdapter("select * from tbl_UserMaster where UserCode='" + UserName + "'  ", con);
                Das.Fill(dt);
                 OrderUser = dt.Rows[0]["Username"].ToString();
            }
            //  string imageURL = Server.MapPath("~") + "/Content/Img/pune_abrassiv_logo.jpg";
            // string imageURLL = Server.MapPath("~") + "/image/Tirupati-Stamp_Final.png";

            // iTextSharp.text.Image png = iTextSharp.text.Image.GetInstance(imageURL);
            // iTextSharp.text.Image pngg = iTextSharp.text.Image.GetInstance(imageURLL);

            //Resize image depend upon your need

            //png.ScaleToFit(100, 70);
            //// pngg.ScaleToFit(100, 50);

            ////For Image Position
            //png.SetAbsolutePosition(30, 740);
            ////  pngg.SetAbsolutePosition(440, 160);
            ////var document = new Document();

            ////Give space before image
            ////png.ScaleToFit(document.PageSize.Width - (document.RightMargin * 100), 50);
            //png.SpacingBefore = 50f;
            //// pngg.SpacingBefore = 50f;

            ////Give some space after the image

            //png.SpacingAfter = 1f;
            ////  pngg.SpacingAfter = 1f;

            //png.Alignment = Element.ALIGN_LEFT;
            ////  pngg.Alignment = Element.ALIGN_LEFT;

            ////paragraphimage.Add(png);                                                                                                                                              
            ////doc.Add(paragraphimage);
            //doc.Add(png);
            //doc.Add(pngg);

            PdfContentByte cb = writer.DirectContent;

            cb.Rectangle(17f, 735f, 560f, 80f);
            cb.Stroke();

            // Header 
            //cb.BeginText();

            ////cdd.SetColorFill (BaseColor.RED);

            //cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 25);
            //cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Pune Abrasive Pvt. Ltd.", 165, 790, 0);
            //cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 11);
            //cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Plot No. 84, 2nd Floor D2 Block, MIDC Chinchwad, KSB Chowk,Near Shell Petrol Pump,", 130, 775, 0);
            //cb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 11);
            //cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, " Pune 411019, Maharashtra,India", 200, 763, 0);
            //cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "Website:http://www.puneabrasives.com/", 200, 753, 0);
            //cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "GST No.: 27ABCCS7002A1ZW   |   PAN No.: **NFP34***", 160, 740, 0); cb.EndText();

            //PdfContentByte cbb = writer.DirectContent;
            //cbb.Rectangle(17f, 710f, 560f, 25f);
            //cbb.Stroke();
            //// Header 
            //cbb.BeginText();
            //cbb.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 11);
            //cbb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, " girish.kulkarni@puneabrasives.com                                                                                 +91 9860441689, 9511712429 ", 30, 720, 0);
            //cbb.EndText();

            PdfContentByte cd = writer.DirectContent;
            //cd.Rectangle(17f, 680f, 560f, 25f);
            cd.Stroke();
            // Header 
            cd.BeginText();
            cd.SetFontAndSize(BaseFont.CreateFont(@"C:\Windows\Fonts\Calibrib.ttf", "Identity-H", BaseFont.EMBEDDED), 17);
            cd.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "ORDER ACCEPTANCE", 200, 800, 0);
            cd.EndText();

            if (Dt.Rows.Count > 0)
            {
                var CreateDate = DateTime.Now.ToString("dd-MM-yyyy");
                string CustomerName = Dt.Rows[0]["CustomerName"].ToString();
                string KindAtt = Dt.Rows[0]["KindAtt"].ToString();
                string Pono = Dt.Rows[0]["Pono"].ToString();
                string SerialNo = Dt.Rows[0]["SerialNo"].ToString();
                string PoDate = Dt.Rows[0]["PoDate"].ToString().TrimEnd("0:0".ToCharArray());
                string Deliverydate = Dt.Rows[0]["Deliverydate"].ToString().TrimEnd("0:0".ToCharArray());
                string Mobileno = Dt.Rows[0]["Mobileno"].ToString();
                string Billingaddress = Dt.Rows[0]["BillingAddress"].ToString();
                string Shippingaddress = Dt.Rows[0]["ShippingAddress"].ToString();
                string CreatedDate = Dt.Rows[0]["CreatedOn"].ToString().TrimEnd("0:0".ToCharArray());
                string TotalAmount = Dt.Rows[0]["Total_Price"].ToString();
                string AmountInWord = Dt.Rows[0]["Totalinword"].ToString();
                string GSTNo = Dt.Rows[0]["GSTNo"].ToString();
                string PANNo = Dt.Rows[0]["PANNo"].ToString();
                string VendorCode = Dt.Rows[0]["VendorCode"].ToString();
               
                Paragraph paragraphTable1 = new Paragraph();
                paragraphTable1.SpacingBefore = 100f;
                paragraphTable1.SpacingAfter = 10f;

                PdfPTable table = new PdfPTable(4);

                float[] widths2 = new float[] { 100, 180, 100, 180 };
                table.SetWidths(widths2);
                table.TotalWidth = 560f;
                table.LockedWidth = true;

                DateTime ffff1 = Convert.ToDateTime(Dt.Rows[0]["PoDate"].ToString());
                string datee = ffff1.ToString("dd-MM-yyyy");

                DateTime ffff2 = Convert.ToDateTime(Dt.Rows[0]["CreatedOn"].ToString());
                string dateee = ffff2.ToString("dd-MM-yyyy");

                table.AddCell(new Phrase("   Customer Name : ", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("   " + CustomerName, FontFactory.GetFont("Arial", 9, Font.BOLD)));

                table.AddCell(new Phrase("   OA No : ", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("   " + Pono, FontFactory.GetFont("Arial", 9, Font.BOLD)));

                table.AddCell(new Phrase("   Kind Att. : ", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("   " + KindAtt, FontFactory.GetFont("Arial", 9, Font.BOLD)));

                table.AddCell(new Phrase("   Delivery Date : ", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("   " + Deliverydate, FontFactory.GetFont("Arial", 9, Font.BOLD)));

                //table.AddCell(new Phrase("   Mobile No. : ", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                //table.AddCell(new Phrase("   " + Mobileno, FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("   GST No. : ", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("   " + GSTNo, FontFactory.GetFont("Arial", 9, Font.BOLD)));

                table.AddCell(new Phrase("   PAN No. : ", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("   " + PANNo, FontFactory.GetFont("Arial", 9, Font.BOLD)));

                table.AddCell(new Phrase("   Billing Address : ", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("   " + Billingaddress, FontFactory.GetFont("Arial", 9, Font.BOLD)));

                table.AddCell(new Phrase("   Shipping Address : ", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("   " + Shippingaddress, FontFactory.GetFont("Arial", 9, Font.BOLD)));

                table.AddCell(new Phrase("   Serial No ", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("   " + SerialNo, FontFactory.GetFont("Arial", 9, Font.BOLD)));

                table.AddCell(new Phrase("  VendorCode  ", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                table.AddCell(new Phrase("   " + VendorCode, FontFactory.GetFont("Arial", 9, Font.BOLD)));



                paragraphTable1.Add(table);
                doc.Add(paragraphTable1);

                Paragraph paragraphTable2 = new Paragraph();
                paragraphTable2.SpacingAfter = 0f;
                table = new PdfPTable(12);
                float[] widths3 = new float[] { 1.5f, 8f, 3f, 2f, 2f, 3f, 3f, 3f, 2f, 2f, 2f, 3f };
                table.SetWidths(widths3);

                //------------HEADER END-----------------

                SqlDataAdapter Daa = new SqlDataAdapter("SELECT distinct(Productname),[ID],[Pono],[Description],[HSN],[Quantity],[Units],[Rate],[CGSTPer],[CGSTAmt],[SGSTPer],[SGSTAmt],[IGSTPer],[IGSTAmt],[Total],[Discountpercentage],[DiscountAmount],[Alltotal] FROM [tbl_CustomerPurchaseOrderDtls]  WHERE Pono ='" + PO_NO + "'  ", con);
                DataTable Dtt = new DataTable();
                Daa.Fill(Dtt);

                decimal DiscPercentage = Convert.ToDecimal(Dtt.Rows[0]["Discountpercentage"]);

                decimal CGSTAmt = Convert.ToDecimal(Dtt.Rows[0]["CGSTAmt"]);
                decimal SGSTAmt = Convert.ToDecimal(Dtt.Rows[0]["SGSTAmt"]);
                decimal IGSTAmt = Convert.ToDecimal(Dtt.Rows[0]["IGSTAmt"]);

                decimal Basic = Convert.ToDecimal(Dtt.Rows[0]["Total"]);
                var Disc = Basic * DiscPercentage / 100;
                double Ttotal_price = 0, CGST_Total = 0, SGST_Total = 0, IGST_Total = 0;

                var Taxable = Basic - Disc;

                if (Dt.Rows.Count > 0)
                {
                    SqlDataAdapter Daaa = new SqlDataAdapter("SELECT distinct(Productname) FROM [tbl_CustomerPurchaseOrderDtls]  WHERE Pono ='" + PO_NO + "'  ", con);
                    DataTable Dttt = new DataTable();
                    Daaa.Fill(Dttt);
                    table.TotalWidth = 560f;
                    table.LockedWidth = true;
                    table.AddCell(new Phrase(" No.", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase("       Item & Description", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase("    HSN", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase("  Qty", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase(" Unit", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase("    Rate", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase("Discount Amount", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase(" Taxable", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase("CGST", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase("SGST", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase("IGST", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    table.AddCell(new Phrase(" Amount", FontFactory.GetFont("Arial", 10, Font.BOLD)));

                    int rowid = 1;
                    foreach (DataRow dr in Dtt.Rows)
                    {

                        table.TotalWidth = 560f;
                        table.LockedWidth = true;

                        table.AddCell(new Phrase("   " + rowid.ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase("" + dr["Productname"].ToString() + "\n\n" + "" + dr["Description"].ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase("     " + dr["HSN"].ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase("     " + dr["Quantity"].ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase(" " + dr["Units"].ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase("   " + dr["Rate"].ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase("     " + dr["DiscountAmount"].ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase("     " + dr["Total"].ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase("     " + dr["CGSTPer"].ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase("     " + dr["SGSTPer"].ToString(), FontFactory.GetFont("Arial", 9)));
                        table.AddCell(new Phrase("     " + dr["IGSTPer"].ToString(), FontFactory.GetFont("Arial", 9)));
                        rowid++;
                        table.AddCell(new Phrase("   " + dr["Alltotal"].ToString(), FontFactory.GetFont("Arial", 9)));

                        Ttotal_price += Convert.ToDouble(dr["Total"].ToString());

                        CGST_Total += Convert.ToDouble(dr["CGSTAmt"].ToString());
                        SGST_Total += Convert.ToDouble(dr["SGSTAmt"].ToString());
                        IGST_Total += Convert.ToDouble(dr["IGSTAmt"].ToString());


                    }
                }
                paragraphTable2.Add(table);
                doc.Add(paragraphTable2);

                Font font12 = FontFactory.GetFont("Arial", 12, Font.BOLD);
                Font font10 = FontFactory.GetFont("Arial", 10, Font.BOLD);
                Paragraph paragraph = new Paragraph("", font12);

                table = new PdfPTable(12);
                table.TotalWidth = 560f;
                table.LockedWidth = true;
                table.SetWidths(new float[] { 1.5f, 8f, 3f, 2f, 2f, 3f, 3f, 3f, 2f, 2f, 2f, 3f });
                table.AddCell(paragraph);
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 20, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 20, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 20, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 20, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 20, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 20, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 20, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 20, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 20, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 20, Font.BOLD)));
                table.AddCell(new Phrase("", FontFactory.GetFont("Arial", 20, Font.BOLD)));


                table.AddCell(new Phrase("  \n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("  \n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("  \n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("  \n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("  \n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("  \n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("  \n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("  \n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("  \n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("  \n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("  \n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("  \n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("  \n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                table.AddCell(new Phrase("  \n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n", FontFactory.GetFont("Arial", 10, Font.BOLD)));

                //  }
                doc.Add(table);

                //new code 
                Paragraph paragraphTable9 = new Paragraph();

                table = new PdfPTable(3);
                table.TotalWidth = 560f;
                table.LockedWidth = true;

                table.SetWidths(new float[] { 0f, 66f, 12f });
                table.AddCell(paragraph);
                PdfPCell celll2tt = new PdfPCell(new Phrase("SubTotal", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                celll2tt.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(celll2tt);
                PdfPCell celll3tt = new PdfPCell(new Phrase(Ttotal_price.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD)));
                celll3tt.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(celll3tt);
                doc.Add(table);


                Paragraph paragraphTable6 = new Paragraph();

                table = new PdfPTable(3);
                table.TotalWidth = 560f;
                table.LockedWidth = true;

                table.SetWidths(new float[] { 0f, 66f, 12f });
                table.AddCell(paragraph);
                PdfPCell cell2tt = new PdfPCell(new Phrase("CGST", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                cell2tt.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(cell2tt);
                PdfPCell cell3tt = new PdfPCell(new Phrase(CGST_Total.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD)));
                cell3tt.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(cell3tt);
                doc.Add(table);

                Paragraph paragraphTable7 = new Paragraph();

                table = new PdfPTable(3);
                table.TotalWidth = 560f;
                table.LockedWidth = true;

                table.SetWidths(new float[] { 0f, 66f, 12f });
                table.AddCell(paragraph);
                PdfPCell cell2ttt = new PdfPCell(new Phrase("SGST", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                cell2ttt.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(cell2ttt);
                PdfPCell cell3ttt = new PdfPCell(new Phrase(SGST_Total.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD)));
                cell3ttt.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(cell3ttt);
                doc.Add(table);

                Paragraph paragraphTable8 = new Paragraph();

                table = new PdfPTable(3);
                table.TotalWidth = 560f;
                table.LockedWidth = true;

                table.SetWidths(new float[] { 0f, 66f, 12f });
                table.AddCell(paragraph);
                PdfPCell cell2tttt = new PdfPCell(new Phrase("IGST", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                cell2tttt.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(cell2tttt);
                PdfPCell cell3tttt = new PdfPCell(new Phrase(IGST_Total.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD)));
                cell3tttt.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(cell3tttt);
                doc.Add(table);

                //new code end

                //Add Total Row start
                Paragraph paragraphTable5 = new Paragraph();

                table = new PdfPTable(3);
                table.TotalWidth = 560f;
                table.LockedWidth = true;

                table.SetWidths(new float[] { 0f, 66f, 12f });
                table.AddCell(paragraph);
                PdfPCell cell2t = new PdfPCell(new Phrase("Grand Total", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                cell2t.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(cell2t);
                PdfPCell cell3t = new PdfPCell(new Phrase(TotalAmount.ToString(), FontFactory.GetFont("Arial", 10, Font.BOLD)));
                cell3t.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.AddCell(cell3t);
                doc.Add(table);


                table = new PdfPTable(3);
                table.TotalWidth = 560f;
                table.LockedWidth = true;
                table.SetWidths(new float[] { 0f, 25f, 63f });
                table.AddCell(paragraph);
                PdfPCell cell66 = new PdfPCell(new Phrase("Amount In Words Rs. ", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                cell66.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell66);
                PdfPCell cell77 = new PdfPCell(new Phrase(AmountInWord, FontFactory.GetFont("Arial", 10, Font.BOLD)));
                cell77.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell77);
                doc.Add(table);

                table = new PdfPTable(2);
                table.TotalWidth = 560f;
                table.LockedWidth = true;
                table.SetWidths(new float[] { 310f, 250f });

                //table.AddCell(paragraph11);
                // table.AddCell(new Phrase("\b\n\n\b\n\n\b                                    Receiver's Signature ", FontFactory.GetFont("Arial", 10)));
                PdfPCell receiverSignatureCell = new PdfPCell();

                receiverSignatureCell.AddElement(new Phrase("\b\n\n\b\n                                   Receiver's Signature ", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                receiverSignatureCell.AddElement(new Phrase("\b\b\b\b\b\b\b\b\b\b \b\b\b\b                      " + OrderUser, FontFactory.GetFont("Arial", 10)));
                table.AddCell(receiverSignatureCell);
                table.AddCell(new Phrase("\b      For,\n\n\b\b\b\b\b\b\b\b\b             Pune Abrasive Pvt. Ltd. \n\n\n\n\n                          Authorised Signatory", FontFactory.GetFont("Arial", 11, Font.BOLD)));

                doc.Add(table);
            }
            doc.Close();
            ifrRight6.Attributes["src"] = @"../PDF_Files/" + "OA3.pdf";

        }
        catch (Exception ex)
        {

            throw ex;
        }

    }
}