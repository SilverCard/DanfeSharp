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
    <summary>Mac OS standard latin encoding [PDF:1.6:D].</summary>
  */
  internal sealed class MacRomanEncoding
    : Encoding
  {
    public MacRomanEncoding(
      )
    {
      Put(65,"A");
      Put(174,"AE");
      Put(231,"Aacute");
      Put(229,"Acircumflex");
      Put(128,"Adieresis");
      Put(203,"Agrave");
      Put(129,"Aring");
      Put(204,"Atilde");
      Put(66,"B");
      Put(67,"C");
      Put(130,"Ccedilla");
      Put(68,"D");
      Put(69,"E");
      Put(131,"Eacute");
      Put(230,"Ecircumflex");
      Put(232,"Edieresis");
      Put(233,"Egrave");
      Put(70,"F");
      Put(71,"G");
      Put(72,"H");
      Put(73,"I");
      Put(234,"Iacute");
      Put(235,"Icircumflex");
      Put(236,"Idieresis");
      Put(237,"Igrave");
      Put(74,"J");
      Put(75,"K");
      Put(76,"L");
      Put(77,"M");
      Put(78,"N");
      Put(132,"Ntilde");
      Put(79,"O");
      Put(206,"OE");
      Put(238,"Oacute");
      Put(239,"Ocircumflex");
      Put(133,"Odieresis");
      Put(241,"Ograve");
      Put(175,"Oslash");
      Put(205,"Otilde");
      Put(80,"P");
      Put(81,"Q");
      Put(82,"R");
      Put(83,"S");
      Put(84,"T");
      Put(85,"U");
      Put(242,"Uacute");
      Put(243,"Ucircumflex");
      Put(134,"Udieresis");
      Put(244,"Ugrave");
      Put(86,"V");
      Put(87,"W");
      Put(88,"X");
      Put(89,"Y");
      Put(217,"Ydieresis");
      Put(90,"Z");
      Put(97,"a");
      Put(135,"aacute");
      Put(137,"acircumflex");
      Put(171,"acute");
      Put(138,"adieresis");
      Put(190,"ae");
      Put(136,"agrave");
      Put(38,"ampersand");
      Put(140,"aring");
      Put(94,"asciicircum");
      Put(126,"asciitilde");
      Put(42,"asterisk");
      Put(64,"at");
      Put(139,"atilde");
      Put(98,"b");
      Put(92,"backslash");
      Put(124,"bar");
      Put(123,"braceleft");
      Put(125,"braceright");
      Put(91,"bracketleft");
      Put(93,"bracketright");
      Put(249,"breve");
      Put(165,"bullet");
      Put(99,"c");
      Put(255,"caron");
      Put(141,"ccedilla");
      Put(252,"cedilla");
      Put(162,"cent");
      Put(246,"circumflex");
      Put(58,"colon");
      Put(44,"comma");
      Put(169,"copyright");
      Put(219,"currency");
      Put(100,"d");
      Put(160,"dagger");
      Put(224,"daggerdbl");
      Put(161,"degree");
      Put(172,"dieresis");
      Put(214,"divide");
      Put(36,"dollar");
      Put(250,"dotaccent");
      Put(245,"dotlessi");
      Put(101,"e");
      Put(142,"eacute");
      Put(144,"ecircumflex");
      Put(145,"edieresis");
      Put(143,"egrave");
      Put(56,"eight");
      Put(201,"ellipsis");
      Put(209,"emdash");
      Put(208,"endash");
      Put(61,"equal");
      Put(33,"exclam");
      Put(193,"exclamdown");
      Put(102,"f");
      Put(222,"fi");
      Put(53,"five");
      Put(223,"fl");
      Put(196,"florin");
      Put(52,"four");
      Put(218,"fraction");
      Put(103,"g");
      Put(167,"germandbls");
      Put(96,"grave");
      Put(62,"greater");
      Put(199,"guillemotleft");
      Put(200,"guillemotright");
      Put(220,"guilsinglleft");
      Put(221,"guilsinglright");
      Put(104,"h");
      Put(253,"hungarumlaut");
      Put(45,"hyphen");
      Put(105,"i");
      Put(146,"iacute");
      Put(148,"icircumflex");
      Put(149,"idieresis");
      Put(147,"igrave");
      Put(106,"j");
      Put(107,"k");
      Put(108,"l");
      Put(60,"less");
      Put(194,"logicalnot");
      Put(109,"m");
      Put(248,"macron");
      Put(181,"mu");
      Put(110,"n");
      Put(57,"nine");
      Put(150,"ntilde");
      Put(35,"numbersign");
      Put(111,"o");
      Put(151,"oacute");
      Put(153,"ocircumflex");
      Put(154,"odieresis");
      Put(207,"oe");
      Put(254,"ogonek");
      Put(152,"ograve");
      Put(49,"one");
      Put(187,"ordfeminine");
      Put(188,"ordmasculine");
      Put(191,"oslash");
      Put(155,"otilde");
      Put(112,"p");
      Put(166,"paragraph");
      Put(40,"parenleft");
      Put(41,"parenright");
      Put(37,"percent");
      Put(46,"period");
      Put(225,"periodcentered");
      Put(228,"perthousand");
      Put(43,"plus");
      Put(177,"plusminus");
      Put(113,"q");
      Put(63,"question");
      Put(192,"questiondown");
      Put(34,"quotedbl");
      Put(227,"quotedblbase");
      Put(210,"quotedblleft");
      Put(211,"quotedblright");
      Put(212,"quoteleft");
      Put(213,"quoteright");
      Put(226,"quotesinglbase");
      Put(39,"quotesingle");
      Put(114,"r");
      Put(168,"registered");
      Put(251,"ring");
      Put(115,"s");
      Put(164,"section");
      Put(59,"semicolon");
      Put(55,"seven");
      Put(54,"six");
      Put(47,"slash");
      Put(32,"space");
      Put(163,"sterling");
      Put(116,"t");
      Put(51,"three");
      Put(247,"tilde");
      Put(170,"trademark");
      Put(50,"two");
      Put(117,"u");
      Put(156,"uacute");
      Put(158,"ucircumflex");
      Put(159,"udieresis");
      Put(157,"ugrave");
      Put(95,"underscore");
      Put(118,"v");
      Put(119,"w");
      Put(120,"x");
      Put(121,"y");
      Put(216,"ydieresis");
      Put(180,"yen");
      Put(122,"z");
      Put(48,"zero");
    }
  }
}