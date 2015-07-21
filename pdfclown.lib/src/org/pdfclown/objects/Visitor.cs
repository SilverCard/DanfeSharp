/*
  Copyright 2012 Stefano Chizzolini. http://www.pdfclown.org

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

using org.pdfclown.tokens;

using System.Collections.Generic;

namespace org.pdfclown.objects
{
  /**
    <summary>Visitor object.</summary>
  */
  public class Visitor
    : IVisitor
  {
    public virtual PdfObject Visit(
      ObjectStream obj,
      object data
      )
    {
      foreach(PdfDataObject value in obj.Values)
      {value.Accept(this, data);}
      return obj;
    }

    public virtual PdfObject Visit(
      PdfArray obj,
      object data
      )
    {
      foreach(PdfDirectObject item in obj)
      {
        if(item != null)
        {item.Accept(this, data);}
      }
      return obj;
    }

    public virtual PdfObject Visit(
      PdfBoolean obj,
      object data
      )
    {return obj;}

    public PdfObject Visit(
      PdfDataObject obj,
      object data
      )
    {return obj.Accept(this, data);}

    public virtual PdfObject Visit(
      PdfDate obj,
      object data
      )
    {return obj;}

    public virtual PdfObject Visit(
      PdfDictionary obj,
      object data
      )
    {
      foreach(PdfDirectObject value in obj.Values)
      {
        if(value != null)
        {value.Accept(this, data);}
      }
      return obj;
    }

    public virtual PdfObject Visit(
      PdfIndirectObject obj,
      object data
      )
    {
      PdfDataObject dataObject = obj.DataObject;
      if(dataObject != null)
      {dataObject.Accept(this, data);}
      return obj;
    }

    public virtual PdfObject Visit(
      PdfInteger obj,
      object data
      )
    {return obj;}

    public virtual PdfObject Visit(
      PdfName obj,
      object data
      )
    {return obj;}

    public virtual PdfObject Visit(
      PdfReal obj,
      object data
      )
    {return obj;}

    public virtual PdfObject Visit(
      PdfReference obj,
      object data
      )
    {
      obj.IndirectObject.Accept(this, data);
      return obj;
    }

    public virtual PdfObject Visit(
      PdfStream obj,
      object data
      )
    {return obj;}

    public virtual PdfObject Visit(
      PdfString obj,
      object data
      )
    {return obj;}

    public virtual PdfObject Visit(
      PdfTextString obj,
      object data
      )
    {return obj;}

    public virtual PdfObject Visit(
      XRefStream obj,
      object data
      )
    {return obj;}
  }
}