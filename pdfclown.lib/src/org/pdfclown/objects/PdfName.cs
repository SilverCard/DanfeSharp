/*
  Copyright 2006-2012 Stefano Chizzolini. http://www.pdfclown.org

  Contributors:
    * Stefano Chizzolini (original code developer, http://www.stefanochizzolini.it)

  This file should be part of the source code distribution of "PDF Clown library" (the
  Program): see the accompanying README files for more info.

  This Program is free software; you can redistribute it and/or modify it under the terms
  of the GNU Lesser General Public License as published by the Free Software Foundation;
  either version 3 of the License, or (at your option) any later version.

  This Program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY,
  either expressed or implied; without even the implied warranty of MERCHANTABILITY or
  FITNESS FOR A PARTICULAR PURPOSE. See the License for more details.

  You should have received a copy of the GNU Lesser General Public License along with this
  Program (see README files); if not, go to the GNU website (http://www.gnu.org/licenses/).

  Redistribution and use, with or without modification, are permitted provided that such
  redistributions retain the above copyright notice, license and disclaimer, along with
  this list of conditions.
*/

using org.pdfclown.bytes;
using org.pdfclown.files;
using tokens = org.pdfclown.tokens;

using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace org.pdfclown.objects
{
  /**
    <summary>PDF name object [PDF:1.6:3.2.4].</summary>
  */
  public sealed class PdfName
    : PdfSimpleObject<string>
  {
    /*
      NOTE: As name objects are simple symbols uniquely defined by sequences of characters,
      the bytes making up the name are never treated as text, always keeping them escaped.
    */
    #region static
    #region fields
    /*
      NOTE: Name lexical conventions prescribe that the following reserved characters
      are to be escaped when placed inside names' character sequences:
        - delimiters;
        - whitespaces;
        - '#' (number sign character).
    */
    private static readonly Regex EscapedPattern = new Regex("#([\\da-fA-F]{2})");
    private static readonly Regex UnescapedPattern = new Regex("[\\s\\(\\)<>\\[\\]{}/%#]");

    #pragma warning disable 0108
    public static readonly PdfName A = new PdfName("A");
    public static readonly PdfName a = new PdfName("a");
    public static readonly PdfName A85 = new PdfName("A85");
    public static readonly PdfName AA = new PdfName("AA");
    public static readonly PdfName AC = new PdfName("AC");
    public static readonly PdfName Action = new PdfName("Action");
    public static readonly PdfName AcroForm = new PdfName("AcroForm");
    public static readonly PdfName AHx = new PdfName("AHx");
    public static readonly PdfName AIS = new PdfName("AIS");
    public static readonly PdfName All = new PdfName("All");
    public static readonly PdfName AllOff = new PdfName("AllOff");
    public static readonly PdfName AllOn = new PdfName("AllOn");
    public static readonly PdfName AllPages = new PdfName("AllPages");
    public static readonly PdfName AN = new PdfName("AN");
    public static readonly PdfName Annot = new PdfName("Annot");
    public static readonly PdfName Annotation = new PdfName("Annotation");
    public static readonly PdfName Annots = new PdfName("Annots");
    public static readonly PdfName AnyOff = new PdfName("AnyOff");
    public static readonly PdfName AnyOn = new PdfName("AnyOn");
    public static readonly PdfName AP = new PdfName("AP");
    public static readonly PdfName Approved = new PdfName("Approved");
    public static readonly PdfName ArtBox = new PdfName("ArtBox");
    public static readonly PdfName AS = new PdfName("AS");
    public static readonly PdfName Ascent = new PdfName("Ascent");
    public static readonly PdfName ASCII85Decode = new PdfName("ASCII85Decode");
    public static readonly PdfName ASCIIHexDecode = new PdfName("ASCIIHexDecode");
    public static readonly PdfName AsIs = new PdfName("AsIs");
    public static readonly PdfName Author = new PdfName("Author");
    public static readonly PdfName B = new PdfName("B");
    public static readonly PdfName BaseEncoding = new PdfName("BaseEncoding");
    public static readonly PdfName BaseFont = new PdfName("BaseFont");
    public static readonly PdfName BaseState = new PdfName("BaseState");
    public static readonly PdfName BBox = new PdfName("BBox");
    public static readonly PdfName BC = new PdfName("BC");
    public static readonly PdfName BE = new PdfName("BE");
    public static readonly PdfName Bead = new PdfName("Bead");
    public static readonly PdfName BG = new PdfName("BG");
    public static readonly PdfName BitsPerComponent = new PdfName("BitsPerComponent");
    public static readonly PdfName BitsPerSample = new PdfName("BitsPerSample");
    public static readonly PdfName Bl = new PdfName("Bl");
    public static readonly PdfName BlackPoint = new PdfName("BlackPoint");
    public static readonly PdfName BleedBox = new PdfName("BleedBox");
    public static readonly PdfName Blinds = new PdfName("Blinds");
    public static readonly PdfName BM = new PdfName("BM");
    public static readonly PdfName Border = new PdfName("Border");
    public static readonly PdfName Bounds = new PdfName("Bounds");
    public static readonly PdfName Box = new PdfName("Box");
    public static readonly PdfName BPC = new PdfName("BPC");
    public static readonly PdfName BS = new PdfName("BS");
    public static readonly PdfName Btn = new PdfName("Btn");
    public static readonly PdfName BU = new PdfName("BU");
    public static readonly PdfName Butt = new PdfName("Butt");
    public static readonly PdfName C = new PdfName("C");
    public static readonly PdfName C0 = new PdfName("C0");
    public static readonly PdfName C1 = new PdfName("C1");
    public static readonly PdfName CA = new PdfName("CA");
    public static readonly PdfName ca = new PdfName("ca");
    public static readonly PdfName CalGray = new PdfName("CalGray");
    public static readonly PdfName CalRGB = new PdfName("CalRGB");
    public static readonly PdfName Cap = new PdfName("Cap");
    public static readonly PdfName CapHeight = new PdfName("CapHeight");
    public static readonly PdfName Caret = new PdfName("Caret");
    public static readonly PdfName Catalog = new PdfName("Catalog");
    public static readonly PdfName Category = new PdfName("Category");
    public static readonly PdfName CCF = new PdfName("CCF");
    public static readonly PdfName CCITTFaxDecode = new PdfName("CCITTFaxDecode");
    public static readonly PdfName CenterWindow = new PdfName("CenterWindow");
    public static readonly PdfName Ch = new PdfName("Ch");
    public static readonly PdfName CIDFontType0 = new PdfName("CIDFontType0");
    public static readonly PdfName CIDFontType2 = new PdfName("CIDFontType2");
    public static readonly PdfName CIDSystemInfo = new PdfName("CIDSystemInfo");
    public static readonly PdfName CIDToGIDMap = new PdfName("CIDToGIDMap");
    public static readonly PdfName Circle = new PdfName("Circle");
    public static readonly PdfName CL = new PdfName("CL");
    public static readonly PdfName ClosedArrow = new PdfName("ClosedArrow");
    public static readonly PdfName CMap = new PdfName("CMap");
    public static readonly PdfName CMapName = new PdfName("CMapName");
    public static readonly PdfName Color = new PdfName("Color");
    public static readonly PdfName ColorBurn = new PdfName("ColorBurn");
    public static readonly PdfName ColorDodge = new PdfName("ColorDodge");
    public static readonly PdfName Colors = new PdfName("Colors");
    public static readonly PdfName ColorSpace = new PdfName("ColorSpace");
    public static readonly PdfName Columns = new PdfName("Columns");
    public static readonly PdfName Comment = new PdfName("Comment");
    public static readonly PdfName Confidential = new PdfName("Confidential");
    public static readonly PdfName Configs = new PdfName("Configs");
    public static readonly PdfName Contents = new PdfName("Contents");
    public static readonly PdfName Count = new PdfName("Count");
    public static readonly PdfName Cover = new PdfName("Cover");
    public static readonly PdfName CreationDate = new PdfName("CreationDate");
    public static readonly PdfName Creator = new PdfName("Creator");
    public static readonly PdfName CreatorInfo = new PdfName("CreatorInfo");
    public static readonly PdfName CropBox = new PdfName("CropBox");
    public static readonly PdfName Crypt = new PdfName("Crypt");
    public static readonly PdfName CS = new PdfName("CS");
    public static readonly PdfName CT = new PdfName("CT");
    public static readonly PdfName D = new PdfName("D");
    public static readonly PdfName DA = new PdfName("DA");
    public static readonly PdfName Darken = new PdfName("Darken");
    public static readonly PdfName DC = new PdfName("DC");
    public static readonly PdfName DCT = new PdfName("DCT");
    public static readonly PdfName DCTDecode = new PdfName("DCTDecode");
    public static readonly PdfName Decode = new PdfName("Decode");
    public static readonly PdfName DecodeParms = new PdfName("DecodeParms");
    public static readonly PdfName Departmental = new PdfName("Departmental");
    public static readonly PdfName Desc = new PdfName("Desc");
    public static readonly PdfName DescendantFonts = new PdfName("DescendantFonts");
    public static readonly PdfName Descent = new PdfName("Descent");
    public static readonly PdfName Dest = new PdfName("Dest");
    public static readonly PdfName Dests = new PdfName("Dests");
    public static readonly PdfName DeviceCMYK = new PdfName("DeviceCMYK");
    public static readonly PdfName DeviceGray = new PdfName("DeviceGray");
    public static readonly PdfName DeviceRGB = new PdfName("DeviceRGB");
    public static readonly PdfName DeviceN = new PdfName("DeviceN");
    public static readonly PdfName Di = new PdfName("Di");
    public static readonly PdfName Diamond = new PdfName("Diamond");
    public static readonly PdfName Difference = new PdfName("Difference");
    public static readonly PdfName Differences = new PdfName("Differences");
    public static readonly PdfName Direction = new PdfName("Direction");
    public static readonly PdfName DisplayDocTitle = new PdfName("DisplayDocTitle");
    public static readonly PdfName Dissolve = new PdfName("Dissolve");
    public static readonly PdfName Dm = new PdfName("Dm");
    public static readonly PdfName Domain = new PdfName("Domain");
    public static readonly PdfName DOS = new PdfName("DOS");
    public static readonly PdfName DP = new PdfName("DP");
    public static readonly PdfName DR = new PdfName("DR");
    public static readonly PdfName Draft = new PdfName("Draft");
    public static readonly PdfName DS = new PdfName("DS");
    public static readonly PdfName Dur = new PdfName("Dur");
    public static readonly PdfName DV = new PdfName("DV");
    public static readonly PdfName E = new PdfName("E");
    public static readonly PdfName EF = new PdfName("EF");
    public static readonly PdfName EmbeddedFile = new PdfName("EmbeddedFile");
    public static readonly PdfName EmbeddedFiles = new PdfName("EmbeddedFiles");
    public static readonly PdfName Encode = new PdfName("Encode");
    public static readonly PdfName Encoding = new PdfName("Encoding");
    public static readonly PdfName Encrypt = new PdfName("Encrypt");
    public static readonly PdfName Event = new PdfName("Event");
    public static readonly PdfName Exclusion = new PdfName("Exclusion");
    public static readonly PdfName Experimental = new PdfName("Experimental");
    public static readonly PdfName Expired = new PdfName("Expired");
    public static readonly PdfName Export = new PdfName("Export");
    public static readonly PdfName ExportState = new PdfName("ExportState");
    public static readonly PdfName Extends = new PdfName("Extends");
    public static readonly PdfName ExtGState = new PdfName("ExtGState");
    public static readonly PdfName F = new PdfName("F");
    public static readonly PdfName Fade = new PdfName("Fade");
    public static readonly PdfName FB = new PdfName("FB");
    public static readonly PdfName FDecodeParms = new PdfName("FDecodeParms");
    public static readonly PdfName Ff = new PdfName("Ff");
    public static readonly PdfName FFilter = new PdfName("FFilter");
    public static readonly PdfName Fields = new PdfName("Fields");
    public static readonly PdfName FileAttachment = new PdfName("FileAttachment");
    public static readonly PdfName Filespec = new PdfName("Filespec");
    public static readonly PdfName Filter = new PdfName("Filter");
    public static readonly PdfName Final = new PdfName("Final");
    public static readonly PdfName First = new PdfName("First");
    public static readonly PdfName FirstChar = new PdfName("FirstChar");
    public static readonly PdfName FirstPage = new PdfName("FirstPage");
    public static readonly PdfName Fit = new PdfName("Fit");
    public static readonly PdfName FitB = new PdfName("FitB");
    public static readonly PdfName FitBH = new PdfName("FitBH");
    public static readonly PdfName FitBV = new PdfName("FitBV");
    public static readonly PdfName FitH = new PdfName("FitH");
    public static readonly PdfName FitR = new PdfName("FitR");
    public static readonly PdfName FitV = new PdfName("FitV");
    public static readonly PdfName FitWindow = new PdfName("FitWindow");
    public static readonly PdfName Fl = new PdfName("Fl");
    public static readonly PdfName Flags = new PdfName("Flags");
    public static readonly PdfName FlateDecode = new PdfName("FlateDecode");
    public static readonly PdfName Fly = new PdfName("Fly");
    public static readonly PdfName Fo = new PdfName("Fo");
    public static readonly PdfName Font = new PdfName("Font");
    public static readonly PdfName FontBBox = new PdfName("FontBBox");
    public static readonly PdfName FontDescriptor = new PdfName("FontDescriptor");
    public static readonly PdfName FontFile = new PdfName("FontFile");
    public static readonly PdfName FontFile2 = new PdfName("FontFile2");
    public static readonly PdfName FontFile3 = new PdfName("FontFile3");
    public static readonly PdfName FontName = new PdfName("FontName");
    public static readonly PdfName ForComment = new PdfName("ForComment");
    public static readonly PdfName Form = new PdfName("Form");
    public static readonly PdfName ForPublicRelease = new PdfName("ForPublicRelease");
    public static readonly PdfName FreeText = new PdfName("FreeText");
    public static readonly PdfName FS = new PdfName("FS");
    public static readonly PdfName FT = new PdfName("FT");
    public static readonly PdfName FullScreen = new PdfName("FullScreen");
    public static readonly PdfName Functions = new PdfName("Functions");
    public static readonly PdfName FunctionType = new PdfName("FunctionType");
    public static readonly PdfName FWParams = new PdfName("FWParams");
    public static readonly PdfName Gamma = new PdfName("Gamma");
    public static readonly PdfName Glitter = new PdfName("Glitter");
    public static readonly PdfName GoTo = new PdfName("GoTo");
    public static readonly PdfName GoTo3DView = new PdfName("GoTo3DView");
    public static readonly PdfName GoToAction = new PdfName("GoToAction");
    public static readonly PdfName GoToE = new PdfName("GoToE");
    public static readonly PdfName GoToR = new PdfName("GoToR");
    public static readonly PdfName Graph = new PdfName("Graph");
    public static readonly PdfName H = new PdfName("H");
    public static readonly PdfName HardLight = new PdfName("HardLight");
    public static readonly PdfName Height = new PdfName("Height");
    public static readonly PdfName Help = new PdfName("Help");
    public static readonly PdfName HI = new PdfName("HI");
    public static readonly PdfName Hide = new PdfName("Hide");
    public static readonly PdfName HideMenubar = new PdfName("HideMenubar");
    public static readonly PdfName HideToolbar = new PdfName("HideToolbar");
    public static readonly PdfName HideWindowUI = new PdfName("HideWindowUI");
    public static readonly PdfName Highlight = new PdfName("Highlight");
    public static readonly PdfName Hue = new PdfName("Hue");
    public static readonly PdfName I = new PdfName("I");
    public static readonly PdfName IC = new PdfName("IC");
    public static readonly PdfName ICCBased = new PdfName("ICCBased");
    public static readonly PdfName ID = new PdfName("ID");
    public static readonly PdfName Identity = new PdfName("Identity");
    public static readonly PdfName IdentityH = new PdfName("Identity-H");
    public static readonly PdfName IdentityV = new PdfName("Identity-V");
    public static readonly PdfName IF = new PdfName("IF");
    public static readonly PdfName Image = new PdfName("Image");
    public static readonly PdfName ImportData = new PdfName("ImportData");
    public static readonly PdfName Index = new PdfName("Index");
    public static readonly PdfName Indexed = new PdfName("Indexed");
    public static readonly PdfName Info = new PdfName("Info");
    public static readonly PdfName Ink = new PdfName("Ink");
    public static readonly PdfName InkList = new PdfName("InkList");
    public static readonly PdfName Insert = new PdfName("Insert");
    public static readonly PdfName ItalicAngle = new PdfName("ItalicAngle");
    public static readonly PdfName IX = new PdfName("IX");
    public static readonly PdfName JavaScript = new PdfName("JavaScript");
    public static readonly PdfName JBIG2Decode = new PdfName("JBIG2Decode");
    public static readonly PdfName JPXDecode = new PdfName("JPXDecode");
    public static readonly PdfName JS = new PdfName("JS");
    public static readonly PdfName K = new PdfName("K");
    public static readonly PdfName Key = new PdfName("Key");
    public static readonly PdfName Keywords = new PdfName("Keywords");
    public static readonly PdfName Kids = new PdfName("Kids");
    public static readonly PdfName L = new PdfName("L");
    public static readonly PdfName L2R = new PdfName("L2R");
    public static readonly PdfName Lab = new PdfName("Lab");
    public static readonly PdfName Lang = new PdfName("Lang");
    public static readonly PdfName Language = new PdfName("Language");
    public static readonly PdfName Last = new PdfName("Last");
    public static readonly PdfName LastChar = new PdfName("LastChar");
    public static readonly PdfName LastPage = new PdfName("LastPage");
    public static readonly PdfName Launch = new PdfName("Launch");
    public static readonly PdfName LC = new PdfName("LC");
    public static readonly PdfName LE = new PdfName("LE");
    public static readonly PdfName Leading = new PdfName("Leading");
    public static readonly PdfName Length = new PdfName("Length");
    public static readonly PdfName LI = new PdfName("LI");
    public static readonly PdfName Lighten = new PdfName("Lighten");
    public static readonly PdfName Limits = new PdfName("Limits");
    public static readonly PdfName Line = new PdfName("Line");
    public static readonly PdfName Link = new PdfName("Link");
    public static readonly PdfName ListMode = new PdfName("ListMode");
    public static readonly PdfName LJ = new PdfName("LJ");
    public static readonly PdfName LL = new PdfName("LL");
    public static readonly PdfName LLE = new PdfName("LLE");
    public static readonly PdfName Locked = new PdfName("Locked");
    public static readonly PdfName Luminosity = new PdfName("Luminosity");
    public static readonly PdfName LW = new PdfName("LW");
    public static readonly PdfName LZW = new PdfName("LZW");
    public static readonly PdfName LZWDecode = new PdfName("LZWDecode");
    public static readonly PdfName M = new PdfName("M");
    public static readonly PdfName Mac = new PdfName("Mac");
    public static readonly PdfName MacRomanEncoding = new PdfName("MacRomanEncoding");
    public static readonly PdfName Matrix = new PdfName("Matrix");
    public static readonly PdfName max = new PdfName("max");
    public static readonly PdfName MaxLen = new PdfName("MaxLen");
    public static readonly PdfName MCD = new PdfName("MCD");
    public static readonly PdfName MCS = new PdfName("MCS");
    public static readonly PdfName MediaBox = new PdfName("MediaBox");
    public static readonly PdfName MediaClip = new PdfName("MediaClip");
    public static readonly PdfName MediaDuration = new PdfName("MediaDuration");
    public static readonly PdfName MediaOffset = new PdfName("MediaOffset");
    public static readonly PdfName MediaPlayerInfo = new PdfName("MediaPlayerInfo");
    public static readonly PdfName MediaPlayParams = new PdfName("MediaPlayParams");
    public static readonly PdfName MediaScreenParams = new PdfName("MediaScreenParams");
    public static readonly PdfName Metadata = new PdfName("Metadata");
    public static readonly PdfName MH = new PdfName("MH");
    public static readonly PdfName Mic = new PdfName("Mic");
    public static readonly PdfName min = new PdfName("min");
    public static readonly PdfName MissingWidth = new PdfName("MissingWidth");
    public static readonly PdfName MK = new PdfName("MK");
    public static readonly PdfName ML = new PdfName("ML");
    public static readonly PdfName MMType1 = new PdfName("MMType1");
    public static readonly PdfName ModDate = new PdfName("ModDate");
    public static readonly PdfName Movie = new PdfName("Movie");
    public static readonly PdfName MR = new PdfName("MR");
    public static readonly PdfName MU = new PdfName("MU");
    public static readonly PdfName Multiply = new PdfName("Multiply");
    public static readonly PdfName N = new PdfName("N");
    public static readonly PdfName Name = new PdfName("Name");
    public static readonly PdfName Named = new PdfName("Named");
    public static readonly PdfName Names = new PdfName("Names");
    public static readonly PdfName NewParagraph = new PdfName("NewParagraph");
    public static readonly PdfName NewWindow = new PdfName("NewWindow");
    public static readonly PdfName Next = new PdfName("Next");
    public static readonly PdfName NextPage = new PdfName("NextPage");
    public static readonly PdfName NM = new PdfName("NM");
    public static readonly PdfName None = new PdfName("None");
    public static readonly PdfName Normal = new PdfName("Normal");
    public static readonly PdfName NotApproved = new PdfName("NotApproved");
    public static readonly PdfName Note = new PdfName("Note");
    public static readonly PdfName NotForPublicRelease = new PdfName("NotForPublicRelease");
    public static readonly PdfName NU = new PdfName("NU");
    public static readonly PdfName Nums = new PdfName("Nums");
    public static readonly PdfName O = new PdfName("O");
    public static readonly PdfName ObjStm = new PdfName("ObjStm");
    public static readonly PdfName OC = new PdfName("OC");
    public static readonly PdfName OCG = new PdfName("OCG");
    public static readonly PdfName OCGs = new PdfName("OCGs");
    public static readonly PdfName OCMD = new PdfName("OCMD");
    public static readonly PdfName OCProperties = new PdfName("OCProperties");
    public static readonly PdfName OFF = new PdfName("OFF");
    public static readonly PdfName Off = new PdfName("Off");
    public static readonly PdfName ON = new PdfName("ON");
    public static readonly PdfName OneColumn = new PdfName("OneColumn");
    public static readonly PdfName OP = new PdfName("OP");
    public static readonly PdfName Open = new PdfName("Open");
    public static readonly PdfName OpenAction = new PdfName("OpenAction");
    public static readonly PdfName OpenArrow = new PdfName("OpenArrow");
    public static readonly PdfName OpenType = new PdfName("OpenType");
    public static readonly PdfName Opt = new PdfName("Opt");
    public static readonly PdfName Order = new PdfName("Order");
    public static readonly PdfName Ordering = new PdfName("Ordering");
    public static readonly PdfName OS = new PdfName("OS");
    public static readonly PdfName Outlines = new PdfName("Outlines");
    public static readonly PdfName Overlay = new PdfName("Overlay");
    public static readonly PdfName P = new PdfName("P");
    public static readonly PdfName Page = new PdfName("Page");
    public static readonly PdfName PageLabel = new PdfName("PageLabel");
    public static readonly PdfName PageLabels = new PdfName("PageLabels");
    public static readonly PdfName PageLayout = new PdfName("PageLayout");
    public static readonly PdfName PageMode = new PdfName("PageMode");
    public static readonly PdfName Pages = new PdfName("Pages");
    public static readonly PdfName PaintType = new PdfName("PaintType");
    public static readonly PdfName Paperclip = new PdfName("Paperclip");
    public static readonly PdfName Paragraph = new PdfName("Paragraph");
    public static readonly PdfName Params = new PdfName("Params");
    public static readonly PdfName Parent = new PdfName("Parent");
    public static readonly PdfName Pattern = new PdfName("Pattern");
    public static readonly PdfName PatternType = new PdfName("PatternType");
    public static readonly PdfName PC = new PdfName("PC");
    public static readonly PdfName PDFDocEncoding = new PdfName("PdfDocEncoding");
    public static readonly PdfName PI = new PdfName("PI");
    public static readonly PdfName PID = new PdfName("PID");
    public static readonly PdfName PL = new PdfName("PL");
    public static readonly PdfName PO = new PdfName("PO");
    public static readonly PdfName Polygon = new PdfName("Polygon");
    public static readonly PdfName PolyLine = new PdfName("PolyLine");
    public static readonly PdfName Popup = new PdfName("Popup");
    public static readonly PdfName Predictor = new PdfName("Predictor");
    public static readonly PdfName Prev = new PdfName("Prev");
    public static readonly PdfName PrevPage = new PdfName("PrevPage");
    public static readonly PdfName Print = new PdfName("Print");
    public static readonly PdfName PrintState = new PdfName("PrintState");
    public static readonly PdfName Producer = new PdfName("Producer");
    public static readonly PdfName Properties = new PdfName("Properties");
    public static readonly PdfName Push = new PdfName("Push");
    public static readonly PdfName PushPin = new PdfName("PushPin");
    public static readonly PdfName PV = new PdfName("PV");
    public static readonly PdfName Q = new PdfName("Q");
    public static readonly PdfName QuadPoints = new PdfName("QuadPoints");
    public static readonly PdfName R = new PdfName("R");
    public static readonly PdfName r = new PdfName("r");
    public static readonly PdfName R2L = new PdfName("R2L");
    public static readonly PdfName Range = new PdfName("Range");
    public static readonly PdfName RBGroups = new PdfName("RBGroups");
    public static readonly PdfName RC = new PdfName("RC");
    public static readonly PdfName RClosedArrow = new PdfName("RClosedArrow");
    public static readonly PdfName Rect = new PdfName("Rect");
    public static readonly PdfName Registry = new PdfName("Registry");
    public static readonly PdfName Rendition = new PdfName("Rendition");
    public static readonly PdfName Renditions = new PdfName("Renditions");
    public static readonly PdfName ResetForm = new PdfName("ResetForm");
    public static readonly PdfName Resources = new PdfName("Resources");
    public static readonly PdfName RF = new PdfName("RF");
    public static readonly PdfName RGB = new PdfName("RGB");
    public static readonly PdfName RI = new PdfName("RI");
    public static readonly PdfName RL = new PdfName("RL");
    public static readonly PdfName Root = new PdfName("Root");
    public static readonly PdfName ROpenArrow = new PdfName("ROpenArrow");
    public static readonly PdfName Rotate = new PdfName("Rotate");
    public static readonly PdfName RT = new PdfName("RT");
    public static readonly PdfName RunLengthDecode = new PdfName("RunLengthDecode");
    public static readonly PdfName S = new PdfName("S");
    public static readonly PdfName Saturation = new PdfName("Saturation");
    public static readonly PdfName Screen = new PdfName("Screen");
    public static readonly PdfName Separation = new PdfName("Separation");
    public static readonly PdfName SetOCGState = new PdfName("SetOCGState");
    public static readonly PdfName Shading = new PdfName("Shading");
    public static readonly PdfName Sig = new PdfName("Sig");
    public static readonly PdfName SinglePage = new PdfName("SinglePage");
    public static readonly PdfName Size = new PdfName("Size");
    public static readonly PdfName Slash = new PdfName("Slash");
    public static readonly PdfName SoftLight = new PdfName("SoftLight");
    public static readonly PdfName Sold = new PdfName("Sold");
    public static readonly PdfName Sound = new PdfName("Sound");
    public static readonly PdfName SP = new PdfName("SP");
    public static readonly PdfName Speaker = new PdfName("Speaker");
    public static readonly PdfName Split = new PdfName("Split");
    public static readonly PdfName Square = new PdfName("Square");
    public static readonly PdfName Squiggly = new PdfName("Squiggly");
    public static readonly PdfName SR = new PdfName("SR");
    public static readonly PdfName SS = new PdfName("SS");
    public static readonly PdfName St = new PdfName("St");
    public static readonly PdfName Stamp = new PdfName("Stamp");
    public static readonly PdfName StandardEncoding = new PdfName("StandardEncoding");
    public static readonly PdfName State = new PdfName("State");
    public static readonly PdfName StemV = new PdfName("StemV");
    public static readonly PdfName StrikeOut = new PdfName("StrikeOut");
    public static readonly PdfName StructParent = new PdfName("StructParent");
    public static readonly PdfName Subject = new PdfName("Subject");
    public static readonly PdfName SubmitForm = new PdfName("SubmitForm");
    public static readonly PdfName Subtype = new PdfName("Subtype");
    public static readonly PdfName Supplement = new PdfName("Supplement");
    public static readonly PdfName SW = new PdfName("SW");
    public static readonly PdfName Sy = new PdfName("Sy");
    public static readonly PdfName T = new PdfName("T");
    public static readonly PdfName Tabs = new PdfName("Tabs");
    public static readonly PdfName Tag = new PdfName("Tag");
    public static readonly PdfName Text = new PdfName("Text");
    public static readonly PdfName TF = new PdfName("TF");
    public static readonly PdfName Thread = new PdfName("Thread");
    public static readonly PdfName Threads = new PdfName("Threads");
    public static readonly PdfName TilingType = new PdfName("TilingType");
    public static readonly PdfName Timespan = new PdfName("Timespan");
    public static readonly PdfName Title = new PdfName("Title");
    public static readonly PdfName Toggle = new PdfName("Toggle");
    public static readonly PdfName TopSecret = new PdfName("TopSecret");
    public static readonly PdfName ToUnicode = new PdfName("ToUnicode");
    public static readonly PdfName TP = new PdfName("TP");
    public static readonly PdfName Trans = new PdfName("Trans");
    public static readonly PdfName TrimBox = new PdfName("TrimBox");
    public static readonly PdfName TrueType = new PdfName("TrueType");
    public static readonly PdfName TwoColumnLeft = new PdfName("TwoColumnLeft");
    public static readonly PdfName TwoColumnRight = new PdfName("TwoColumnRight");
    public static readonly PdfName TwoPageLeft = new PdfName("TwoPageLeft");
    public static readonly PdfName TwoPageRight = new PdfName("TwoPageRight");
    public static readonly PdfName Tx = new PdfName("Tx");
    public static readonly PdfName Type = new PdfName("Type");
    public static readonly PdfName Type0 = new PdfName("Type0");
    public static readonly PdfName Type1 = new PdfName("Type1");
    public static readonly PdfName Type1C = new PdfName("Type1C");
    public static readonly PdfName Type3 = new PdfName("Type3");
    public static readonly PdfName U = new PdfName("U");
    public static readonly PdfName UC = new PdfName("UC");
    public static readonly PdfName Unchanged = new PdfName("Unchanged");
    public static readonly PdfName Uncover = new PdfName("Uncover");
    public static readonly PdfName Underline = new PdfName("Underline");
    public static readonly PdfName Unix = new PdfName("Unix");
    public static readonly PdfName URI = new PdfName("URI");
    public static readonly PdfName URL = new PdfName("URL");
    public static readonly PdfName Usage = new PdfName("Usage");
    public static readonly PdfName UseAttachments = new PdfName("UseAttachments");
    public static readonly PdfName UseNone = new PdfName("UseNone");
    public static readonly PdfName UseOC = new PdfName("UseOC");
    public static readonly PdfName UseOutlines = new PdfName("UseOutlines");
    public static readonly PdfName UseThumbs = new PdfName("UseThumbs");
    public static readonly PdfName V = new PdfName("V");
    public static readonly PdfName Version = new PdfName("Version");
    public static readonly PdfName Vertices = new PdfName("Vertices");
    public static readonly PdfName View = new PdfName("View");
    public static readonly PdfName ViewerPreferences = new PdfName("ViewerPreferences");
    public static readonly PdfName ViewState = new PdfName("ViewState");
    public static readonly PdfName VisiblePages = new PdfName("VisiblePages");
    public static readonly PdfName W = new PdfName("W");
    public static readonly PdfName WhitePoint = new PdfName("WhitePoint");
    public static readonly PdfName Widget = new PdfName("Widget");
    public static readonly PdfName Width = new PdfName("Width");
    public static readonly PdfName Widths = new PdfName("Widths");
    public static readonly PdfName Win = new PdfName("Win");
    public static readonly PdfName WinAnsiEncoding = new PdfName("WinAnsiEncoding");
    public static readonly PdfName Wipe = new PdfName("Wipe");
    public static readonly PdfName WP = new PdfName("WP");
    public static readonly PdfName WS = new PdfName("WS");
    public static readonly PdfName X = new PdfName("X");
    public static readonly PdfName XML = new PdfName("XML");
    public static readonly PdfName XObject = new PdfName("XObject");
    public static readonly PdfName XRef = new PdfName("XRef");
    public static readonly PdfName XStep = new PdfName("XStep");
    public static readonly PdfName XYZ = new PdfName("XYZ");
    public static readonly PdfName Yes = new PdfName("Yes");
    public static readonly PdfName YStep = new PdfName("YStep");
    public static readonly PdfName Z = new PdfName("Z");
    public static readonly PdfName Zoom = new PdfName("Zoom");
    #pragma warning restore 0108

    private static readonly byte[] NamePrefixChunk = tokens::Encoding.Pdf.Encode(tokens.Keyword.NamePrefix);
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets the object equivalent to the given value.</summary>
    */
    public static new PdfName Get(
      object value
      )
    {return value == null ? null : Get(value.ToString());}

    /**
      <summary>Gets the object equivalent to the given value.</summary>
    */
    public static PdfName Get(
      string value
      )
    {return value == null ? null : new PdfName(value);}
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region constructors
    public PdfName(
      string value
      ) : this(value, false)
    {}

    internal PdfName(
      string value,
      bool escaped
      )
    {
      /*
        NOTE: To avoid ambiguities due to the presence of '#' characters,
        it's necessary to explicitly state when a name value has already been escaped.
        This is tipically the case of names parsed from a previously-serialized PDF file.
      */
      if(escaped)
      {RawValue = value;}
      else
      {Value = value;}
    }
    #endregion

    #region interface
    #region public
    public override PdfObject Accept(
      IVisitor visitor,
      object data
      )
    {return visitor.Visit(this, data);}

    public override int CompareTo(
      PdfDirectObject obj
      )
    {
      if(!(obj is PdfName))
        throw new ArgumentException("Object MUST be a PdfName");

      return RawValue.CompareTo(((PdfName)obj).RawValue);
    }

    public string StringValue
    {
      get
      {return (string)Value;}
    }

    public override string ToString(
      )
    {
      /*
        NOTE: The textual representation of a name concerns unescaping reserved characters.
      */
      string value = RawValue;
      StringBuilder buffer = new StringBuilder();
      int index = 0;
      Match escapedMatch = EscapedPattern.Match(value);
      while(escapedMatch.Success)
      {
        int start = escapedMatch.Index;
        if(start > index)
        {buffer.Append(value.Substring(index,start-index));}

        buffer.Append(
          (char)Int32.Parse(
            escapedMatch.Groups[1].Value,
            NumberStyles.HexNumber
            )
          );

        index = start + escapedMatch.Length;
        escapedMatch = escapedMatch.NextMatch();
      }
      if(index < value.Length)
      {buffer.Append(value.Substring(index));}

      return buffer.ToString();
    }

    public override object Value
    {
      get
      {return base.Value;}
      protected set
      {
        /*
          NOTE: Before being accepted, any character sequence identifying a name MUST be normalized
          escaping reserved characters.
        */
        StringBuilder buffer = new StringBuilder();
        {
          string stringValue = (string)value;
          int index = 0;
          Match unescapedMatch = UnescapedPattern.Match(stringValue);
          while(unescapedMatch.Success)
          {
            int start = unescapedMatch.Index;
            if(start > index)
            {buffer.Append(stringValue.Substring(index,start-index));}
  
            buffer.Append(
              '#' + String.Format(
                "{0:x}",
                (int)unescapedMatch.Groups[0].Value[0]
                )
              );

            index = start + unescapedMatch.Length;
            unescapedMatch = unescapedMatch.NextMatch();
          }
          if(index < stringValue.Length)
          {buffer.Append(stringValue.Substring(index));}
        }
        RawValue = buffer.ToString();
      }
    }

    public override void WriteTo(
      IOutputStream stream,
      File context
      )
    {stream.Write(NamePrefixChunk); stream.Write(RawValue);}
    #endregion
    #endregion
    #endregion
  }
}