﻿using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DSVAlpin2Lib
{

  public interface IPDFReport
  {
    void Generate();
  }




  public class PDFHelper
  {
    AppDataModel _dm;

    protected List<string> resourcePaths;

    public PDFHelper(AppDataModel dm)
    {
      _dm = dm;

      calcResourcePaths();
    }


    public Image GetImage(string filenameWOExt)
    {
      Image img = null;

      string imgPath = findImage(filenameWOExt);
      if (!string.IsNullOrEmpty(imgPath))
        img = new Image(ImageDataFactory.Create(imgPath));

      return img;
    }


    void calcResourcePaths()
    {
      List<string> paths = new List<string>();
      paths.Add(_dm.GetDB().GetDBPathDirectory());
      paths.Add(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), @"resources\pdf"));

      resourcePaths = paths;
    }

    string findImage(string filenameWOExt)
    {
      foreach (var resDir in resourcePaths)
      {
        try
        {
          var files = Directory.GetFiles(resDir, string.Format("{0}*", filenameWOExt));
          if (files.Length > 0)
            return files[0];
        }
        catch(System.IO.DirectoryNotFoundException)
        {
          continue;
        }
      }
      return null;
    }

  }





  class EndPageHandler : IEventHandler
  {
    PDFHelper _pdfHelper;
    Race _race;
    string _listName;

    string _header1;
    string _footer1;

    public EndPageHandler(PDFHelper pdfHelper, Race race, string listName)
    {
      _pdfHelper = pdfHelper;
      _race = race;
      _listName = listName;

      calculateHeader();
      calculateFooter();
    }



    private void calculateHeader()
    {
      _header1 = _race.Description + "\n\n" + _listName;
    }

    private void calculateFooter()
    {
      Assembly assembly = Assembly.GetEntryAssembly();
      if (assembly == null)
        assembly = Assembly.GetExecutingAssembly();

      if (assembly != null)
      {
        FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
        var companyName = fvi.CompanyName;
        var productName = fvi.ProductName;
        var copyrightYear = fvi.LegalCopyright;
        var productVersion = fvi.ProductVersion;
        var webAddress = "www.race-horology.com";

        _footer1 = string.Format("{0} V{1}, {2} by {3}, {4}", productName, productVersion, copyrightYear, companyName, webAddress);
      }
      else
        _footer1 = "";
    }


    public virtual void HandleEvent(Event @event)
    {
      PdfDocumentEvent docEvent = (PdfDocumentEvent)@event;
      PdfDocument pdfDoc = docEvent.GetDocument();
      PdfPage page = docEvent.GetPage();

      int pageNumber = pdfDoc.GetPageNumber(page);
      Rectangle pageSize = page.GetPageSize();
      PdfCanvas pdfCanvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfDoc);

      ////Set background
      //Color limeColor = new DeviceCmyk(0.208f, 0, 0.584f, 0);
      //Color blueColor = new DeviceCmyk(0.445f, 0.0546f, 0, 0.0667f);
      //pdfCanvas.SaveState()
      //            .SetFillColor(pageNumber % 2 == 1 ? limeColor : blueColor)
      //            .Rectangle(pageSize.GetLeft(), pageSize.GetBottom(), pageSize.GetWidth(), pageSize.GetHeight())
      //            .Fill()
      //            .RestoreState();

      Image logo1 = _pdfHelper.GetImage("Logo1");
      Image logo2 = _pdfHelper.GetImage("Logo2");

      if (logo1 != null)
      {
        //Rectangle area1 = new Rectangle(0, pageSize.GetTop()- pageSize.GetWidth() * 0.2F * logo1.GetImageHeight() / logo1.GetImageWidth(), pageSize.GetWidth() * 0.2F, pageSize.GetWidth() * 0.2F * logo1.GetImageHeight() / logo1.GetImageWidth());
        Rectangle area1 = new Rectangle(24.0F, pageSize.GetTop() - 110.0F - 24.0F, 110.0F, 110.0F);
        new Canvas(pdfCanvas, pdfDoc, area1)
          .SetHorizontalAlignment(HorizontalAlignment.CENTER)
          .Add(logo1);

        //pdfCanvas
        //              .SetStrokeColor(ColorConstants.RED)
        //              .SetLineWidth(0.5f)
        //              .Rectangle(area1)
        //              .Stroke();
      }

      if (logo2 != null)
      {
        //Rectangle area2 = new Rectangle(pageSize.GetRight() - pageSize.GetWidth()*0.2F, pageSize.GetTop() - pageSize.GetWidth() * 0.2F * logo2.GetImageHeight() / logo2.GetImageWidth(), pageSize.GetWidth() * 0.2F, pageSize.GetWidth() * 0.2F*logo2.GetImageHeight()/logo2.GetImageWidth());
        Rectangle area2 = new Rectangle(pageSize.GetRight() - 24.0F - 110.0F, pageSize.GetTop() - 110.0F - 24.0F, 110.0F, 110.0F);
        new Canvas(pdfCanvas, pdfDoc, area2).Add(logo2);

        //pdfCanvas
        //              .SetStrokeColor(ColorConstants.RED)
        //              .SetLineWidth(0.5f)
        //              .Rectangle(area2)
        //              .Stroke();
      }




      Rectangle rect = new Rectangle(24.0F + 110.0F, pageSize.GetTop() - 110.0F - 24.0F, pageSize.GetRight() - 24.0F - 110.0F - 24.0F - 110.0F, 110.0F);
      Paragraph pHead1 = new Paragraph(_header1);
      new Canvas(pdfCanvas, pdfDoc, rect)
              .Add(pHead1
                .SetTextAlignment(TextAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD)).SetFontSize(16)
                );

      pdfCanvas.SetStrokeColor(ColorConstants.BLACK)
               .SetLineWidth(0.3F)
               .MoveTo(24.0F, pageSize.GetTop() - 110.0F - 24.0F - 2.0)
               .LineTo(pageSize.GetRight() - 24.0F, pageSize.GetTop() - 110.0F - 24.0F - 2.0)
               .MoveTo(24.0F, pageSize.GetTop() - 110.0F - 24.0F - 4.0)
               .LineTo(pageSize.GetRight() - 24.0F, pageSize.GetTop() - 110.0F - 24.0F - 4.0)
               .ClosePathStroke();





      // Footer
      Image logo3 = _pdfHelper.GetImage("Logo3");
      if (logo3 != null)
      {
        Rectangle area3 = new Rectangle(24.0F, 24.0F, pageSize.GetWidth() - 2 * 24.0F, (pageSize.GetWidth() - 2 * 24.0F) * logo3.GetImageHeight() / logo3.GetImageWidth());
        new Canvas(pdfCanvas, pdfDoc, area3).Add(logo3);
      }


      pdfCanvas.SetStrokeColor(ColorConstants.BLACK)
               .SetLineWidth(0.3F)
               .MoveTo(24.0F, pageSize.GetBottom() + (pageSize.GetWidth() - 2 * 24.0F) * logo3.GetImageHeight() / logo3.GetImageWidth() + 2 + 24)
               .LineTo(pageSize.GetRight() - 24.0F, +(pageSize.GetWidth() - 2 * 24.0F) * logo3.GetImageHeight() / logo3.GetImageWidth() + 2 + 24)
               .MoveTo(24.0F, pageSize.GetBottom() + (pageSize.GetWidth() - 2 * 24.0F) * logo3.GetImageHeight() / logo3.GetImageWidth() + 4 + 24)
               .LineTo(pageSize.GetRight() - 24.0F, +(pageSize.GetWidth() - 2 * 24.0F) * logo3.GetImageHeight() / logo3.GetImageWidth() + 4 + 24)
               .ClosePathStroke();

      pdfCanvas.SetStrokeColor(ColorConstants.BLACK)
               .SetLineWidth(0.3F)
               .MoveTo(24.0F, pageSize.GetBottom() + (pageSize.GetWidth() - 2 * 24.0F) * logo3.GetImageHeight() / logo3.GetImageWidth() + 2 + 24 + 24)
               .LineTo(pageSize.GetRight() - 24.0F, +(pageSize.GetWidth() - 2 * 24.0F) * logo3.GetImageHeight() / logo3.GetImageWidth() + 2 + 24 + 24)
               .MoveTo(24.0F, pageSize.GetBottom() + (pageSize.GetWidth() - 2 * 24.0F) * logo3.GetImageHeight() / logo3.GetImageWidth() + 4 + 24 + 24)
               .LineTo(pageSize.GetRight() - 24.0F, +(pageSize.GetWidth() - 2 * 24.0F) * logo3.GetImageHeight() / logo3.GetImageWidth() + 4 + 24 + 24)
               .ClosePathStroke();

      Rectangle rect2 = new Rectangle(24.0F, pageSize.GetBottom() + (pageSize.GetWidth() - 2 * 24.0F) * logo3.GetImageHeight() / logo3.GetImageWidth() + 5 + 24, pageSize.GetRight() - 2 * 24.0F, 24.0F);

      Paragraph pFooter1 = new Paragraph(_footer1);
      new Canvas(pdfCanvas, pdfDoc, rect2)
              .Add(pFooter1
                .SetTextAlignment(TextAlignment.CENTER)
                .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA)).SetFontSize(10)
                );



      ////Add header and footer
      //pdfCanvas.BeginText()
      //            .SetFontAndSize(PdfFontFactory.CreateFont(StandardFonts.HELVETICA), 9)
      //            .MoveText(pageSize.GetWidth() / 2 - 60, pageSize.GetTop() - 20)
      //            .ShowText("THE TRUTH IS OUT THERE")
      //            .MoveText(60, -pageSize.GetTop() + 30)
      //            .ShowText(pageNumber.ToString())
      //            .EndText();


      pdfCanvas.Release();
    }



  }




  public abstract class PDFReport : IPDFReport
  {
    Race _race;

    AppDataModel _dm;
    PDFHelper _pdfHelper;

    public PDFReport(Race race)
    {
      _race = race;

      _dm = race.GetDataModel();
      _pdfHelper = new PDFHelper(_dm);
    }

    protected abstract string getTitle();

    protected abstract Table getResultsTable();


    public void Generate()
    {

      var writer = new PdfWriter("test.pdf");
      var pdf = new PdfDocument(writer);

      pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new EndPageHandler(_pdfHelper, _race, getTitle()));


      var document = new Document(pdf, PageSize.A4);

      document.SetMargins(110.0F + 24.0F + 8.0F, 24.0F, 24.0F + 100.0F, 24.0F);

      //document.Add(new Paragraph("Hello World!"));


      Table table = getResultsTable();

      document.Add(table);

      document.Close();

      System.Diagnostics.Process.Start("test.pdf");
    }
  }


  public class RaceRunResultReport : PDFReport
  {
    RaceRun _raceRun;

    public RaceRunResultReport(RaceRun rr) : base(rr.GetRace())
    {
      _raceRun = rr;

      //dm.Location;

      //race.Description;
      //race.DateResult;

      var results = rr.GetResultViewProvider();
    }

    protected override string getTitle()
    {
      return string.Format("ERGEBNISLISTE {0}. Durchgang", _raceRun.Run);
    }


    protected override Table getResultsTable()
    {
      var table = new Table(new float[] { 2, 1, 3, 1 });
      table.SetWidth(UnitValue.CreatePercentValue(100));
      table.SetBorder(Border.NO_BORDER);

      table.AddHeaderCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Text")));

      for (int i = 1; i < 999; i++)
      {
        table.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph(string.Format("{0}", i))));

      }

      return table;
    }


  }


  public class RaceResultReport : PDFReport
  {

    public RaceResultReport(Race race) : base(race)
    {
      //var dm = race.GetDataModel();
      //dm.Location;

      //race.Description;
      //race.DateResult;

      //var results = rr.GetResultViewProvider();
    }

    protected override string getTitle()
    {
      return string.Format("OFFIZIELLE ERGEBNISLISTE");
    }

    protected override Table getResultsTable()
    {
      var table = new Table(new float[] { 2, 1, 3, 1 });
      table.SetWidth(UnitValue.CreatePercentValue(100));
      table.SetBorder(Border.NO_BORDER);

      table.AddHeaderCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph("Text")));

      for (int i = 1; i < 999; i++)
      {
        table.AddCell(new Cell().SetBorder(Border.NO_BORDER).Add(new Paragraph(string.Format("{0}", i))));

      }

      return table;
    }

  }
}
