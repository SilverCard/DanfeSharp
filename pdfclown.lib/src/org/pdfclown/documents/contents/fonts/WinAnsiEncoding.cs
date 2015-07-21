/*
  Copyright 2009-2010 Stefano Chizzolini. http://www.pdfclown.org

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

using org.pdfclown.objects;

using System.Collections.Generic;

namespace org.pdfclown.documents.contents.fonts
{
  /**
    <summary>Windows ANSI encoding (Windows Code Page 1252) [PDF:1.6:D].</summary>
  */
  internal sealed class WinAnsiEncoding
    : Encoding
  {
    public WinAnsiEncoding(
      )
    {
      Put(65,"A");
      Put(198,"AE");
      Put(193,"Aacute");
      Put(194,"Acircumflex");
      Put(196,"Adieresis");
      Put(192,"Agrave");
      Put(197,"Aring");
      Put(195,"Atilde");
      Put(66,"B");
      Put(67,"C");
      Put(199,"Ccedilla");
      Put(68,"D");
      Put(69,"E");
      Put(201,"Eacute");
      Put(202,"Ecircumflex");
      Put(203,"Edieresis");
      Put(200,"Egrave");
      Put(208,"Eth");
      Put(128,"Euro");
      Put(70,"F");
      Put(71,"G");
      Put(72,"H");
      Put(73,"I");
      Put(205,"Iacute");
      Put(206,"Icircumflex");
      Put(207,"Idieresis");
      Put(204,"Igrave");
      Put(74,"J");
      Put(75,"K");
      Put(76,"L");
      Put(77,"M");
      Put(78,"N");
      Put(209,"Ntilde");
      Put(79,"O");
      Put(140,"OE");
      Put(211,"Oacute");
      Put(212,"Ocircumflex");
      Put(214,"Odieresis");
      Put(210,"Ograve");
      Put(216,"Oslash");
      Put(213,"Otilde");
      Put(80,"P");
      Put(81,"Q");
      Put(82,"R");
      Put(83,"S");
      Put(138,"Scaron");
      Put(84,"T");
      Put(222,"Thorn");
      Put(85,"U");
      Put(218,"Uacute");
      Put(219,"Ucircumflex");
      Put(220,"Udieresis");
      Put(217,"Ugrave");
      Put(86,"V");
      Put(87,"W");
      Put(88,"X");
      Put(89,"Y");
      Put(221,"Yacute");
      Put(159,"Ydieresis");
      Put(90,"Z");
      Put(142,"Zcaron");
      Put(97,"a");
      Put(225,"aacute");
      Put(226,"acircumflex");
      Put(180,"acute");
      Put(228,"adieresis");
      Put(230,"ae");
      Put(224,"agrave");
      Put(38,"ampersand");
      Put(229,"aring");
      Put(94,"asciicircum");
      Put(126,"asciitilde");
      Put(42,"asterisk");
      Put(64,"at");
      Put(227,"atilde");
      Put(98,"b");
      Put(92,"backslash");
      Put(124,"bar");
      Put(123,"braceleft");
      Put(125,"braceright");
      Put(91,"bracketleft");
      Put(93,"bracketright");
      Put(166,"brokenbar");
      Put(149,"bullet");
      Put(99,"c");
      Put(231,"ccedilla");
      Put(184,"cedilla");
      Put(162,"cent");
      Put(136,"circumflex");
      Put(58,"colon");
      Put(44,"comma");
      Put(169,"copyright");
      Put(164,"currency");
      Put(100,"d");
      Put(134,"dagger");
      Put(135,"daggerdbl");
      Put(176,"degree");
      Put(168,"dieresis");
      Put(247,"divide");
      Put(36,"dollar");
      Put(101,"e");
      Put(233,"eacute");
      Put(234,"ecircumflex");
      Put(235,"edieresis");
      Put(232,"egrave");
      Put(56,"eight");
      Put(133,"ellipsis");
      Put(151,"emdash");
      Put(150,"endash");
      Put(61,"equal");
      Put(240,"eth");
      Put(33,"exclam");
      Put(161,"exclamdown");
      Put(102,"f");
      Put(53,"five");
      Put(131,"florin");
      Put(52,"four");
      Put(103,"g");
      Put(223,"germandbls");
      Put(96,"grave");
      Put(62,"greater");
      Put(171,"guillemotleft");
      Put(187,"guillemotright");
      Put(139,"guilsinglleft");
      Put(155,"guilsinglright");
      Put(104,"h");
      Put(45,"hyphen");
      Put(105,"i");
      Put(237,"iacute");
      Put(238,"icircumflex");
      Put(239,"idieresis");
      Put(236,"igrave");
      Put(106,"j");
      Put(107,"k");
      Put(108,"l");
      Put(60,"less");
      Put(172,"logicalnot");
      Put(109,"m");
      Put(175,"macron");
      Put(181,"mu");
      Put(215,"multiply");
      Put(110,"n");
      Put(57,"nine");
      Put(241,"ntilde");
      Put(35,"numbersign");
      Put(111,"o");
      Put(243,"oacute");
      Put(244,"ocircumflex");
      Put(246,"odieresis");
      Put(156,"oe");
      Put(242,"ograve");
      Put(49,"one");
      Put(189,"onehalf");
      Put(188,"onequarter");
      Put(185,"onesuperior");
      Put(170,"ordfeminine");
      Put(186,"ordmasculine");
      Put(248,"oslash");
      Put(245,"otilde");
      Put(112,"p");
      Put(182,"paragraph");
      Put(40,"parenleft");
      Put(41,"parenright");
      Put(37,"percent");
      Put(46,"period");
      Put(183,"periodcentered");
      Put(137,"perthousand");
      Put(43,"plus");
      Put(177,"plusminus");
      Put(113,"q");
      Put(63,"question");
      Put(191,"questiondown");
      Put(34,"quotedbl");
      Put(132,"quotedblbase");
      Put(147,"quotedblleft");
      Put(148,"quotedblright");
      Put(145,"quoteleft");
      Put(146,"quoteright");
      Put(130,"quotesinglbase");
      Put(39,"quotesingle");
      Put(114,"r");
      Put(174,"registered");
      Put(115,"s");
      Put(154,"scaron");
      Put(167,"section");
      Put(59,"semicolon");
      Put(55,"seven");
      Put(54,"six");
      Put(47,"slash");
      Put(32,"space");
      Put(163,"sterling");
      Put(116,"t");
      Put(254,"thorn");
      Put(51,"three");
      Put(190,"threequarters");
      Put(179,"threesuperior");
      Put(152,"tilde");
      Put(153,"trademark");
      Put(50,"two");
      Put(178,"twosuperior");
      Put(117,"u");
      Put(250,"uacute");
      Put(251,"ucircumflex");
      Put(252,"udieresis");
      Put(249,"ugrave");
      Put(95,"underscore");
      Put(118,"v");
      Put(119,"w");
      Put(120,"x");
      Put(121,"y");
      Put(253,"yacute");
      Put(255,"ydieresis");
      Put(165,"yen");
      Put(122,"z");
      Put(158,"zcaron");
      Put(48,"zero");
    }
  }
}