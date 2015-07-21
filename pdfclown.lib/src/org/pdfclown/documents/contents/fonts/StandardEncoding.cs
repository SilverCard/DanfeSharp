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
    <summary>Adobe standard Latin-text encoding [PDF:1.6:D].</summary>
  */
  internal sealed class StandardEncoding
    : Encoding
  {
    public StandardEncoding(
      )
    {
      Put(65,"A");
      Put(225,"AE");
      Put(66,"B");
      Put(67,"C");
      Put(68,"D");
      Put(69,"E");
      Put(70,"F");
      Put(71,"G");
      Put(72,"H");
      Put(73,"I");
      Put(74,"J");
      Put(75,"K");
      Put(76,"L");
      Put(232,"Lslash");
      Put(77,"M");
      Put(78,"N");
      Put(79,"O");
      Put(234,"OE");
      Put(233,"Oslash");
      Put(80,"P");
      Put(81,"Q");
      Put(82,"R");
      Put(83,"S");
      Put(84,"T");
      Put(85,"U");
      Put(86,"V");
      Put(87,"W");
      Put(88,"X");
      Put(89,"Y");
      Put(90,"Z");
      Put(97,"a");
      Put(194,"acute");
      Put(241,"ae");
      Put(38,"ampersand");
      Put(94,"asciicircum");
      Put(126,"asciitilde");
      Put(42,"asterisk");
      Put(64,"at");
      Put(98,"b");
      Put(92,"backslash");
      Put(124,"bar");
      Put(123,"braceleft");
      Put(125,"braceright");
      Put(91,"bracketleft");
      Put(93,"bracketright");
      Put(198,"breve");
      Put(183,"bullet");
      Put(99,"c");
      Put(207,"caron");
      Put(203,"cedilla");
      Put(162,"cent");
      Put(195,"circumflex");
      Put(58,"colon");
      Put(44,"comma");
      Put(168,"currency");
      Put(100,"d");
      Put(178,"dagger");
      Put(179,"daggerdbl");
      Put(200,"dieresis");
      Put(36,"dollar");
      Put(199,"dotaccent");
      Put(245,"dotlessi");
      Put(101,"e");
      Put(56,"eight");
      Put(188,"ellipsis");
      Put(208,"emdash");
      Put(177,"endash");
      Put(61,"equal");
      Put(33,"exclam");
      Put(161,"exclamdown");
      Put(102,"f");
      Put(174,"fi");
      Put(53,"five");
      Put(175,"fl");
      Put(166,"florin");
      Put(52,"four");
      Put(164,"fraction");
      Put(103,"g");
      Put(251,"germandbls");
      Put(193,"grave");
      Put(62,"greater");
      Put(171,"guillemotleft");
      Put(187,"guillemotright");
      Put(172,"guilsinglleft");
      Put(173,"guilsinglright");
      Put(104,"h");
      Put(205,"hungarumlaut");
      Put(45,"hyphen");
      Put(105,"i");
      Put(106,"j");
      Put(107,"k");
      Put(108,"l");
      Put(60,"less");
      Put(248,"lslash");
      Put(109,"m");
      Put(197,"macron");
      Put(110,"n");
      Put(57,"nine");
      Put(35,"numbersign");
      Put(111,"o");
      Put(250,"oe");
      Put(206,"ogonek");
      Put(49,"one");
      Put(227,"ordfeminine");
      Put(235,"ordmasculine");
      Put(249,"oslash");
      Put(112,"p");
      Put(182,"paragraph");
      Put(40,"parenleft");
      Put(41,"parenright");
      Put(37,"percent");
      Put(46,"period");
      Put(180,"periodcentered");
      Put(189,"perthousand");
      Put(43,"plus");
      Put(113,"q");
      Put(63,"question");
      Put(191,"questiondown");
      Put(34,"quotedbl");
      Put(185,"quotedblbase");
      Put(170,"quotedblleft");
      Put(186,"quotedblright");
      Put(96,"quoteleft");
      Put(39,"quoteright");
      Put(184,"quotesinglbase");
      Put(169,"quotesingle");
      Put(114,"r");
      Put(202,"ring");
      Put(115,"s");
      Put(167,"section");
      Put(59,"semicolon");
      Put(55,"seven");
      Put(54,"six");
      Put(47,"slash");
      Put(32,"space");
      Put(163,"sterling");
      Put(116,"t");
      Put(51,"three");
      Put(196,"tilde");
      Put(50,"two");
      Put(117,"u");
      Put(95,"underscore");
      Put(118,"v");
      Put(119,"w");
      Put(120,"x");
      Put(121,"y");
      Put(165,"yen");
      Put(122,"z");
      Put(48,"zero");
    }
  }
}