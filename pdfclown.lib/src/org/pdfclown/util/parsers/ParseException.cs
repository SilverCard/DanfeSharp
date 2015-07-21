/*
  Copyright 2011 Stefano Chizzolini. http://www.pdfclown.org

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

using System;

namespace org.pdfclown.util.parsers
{
  /**
    <summary>Exception thrown in case of unexpected condition while parsing.</summary>
  */
  public class ParseException
    : Exception
  {
    #region dynamic
    #region fields
    private long position;
    #endregion

    #region constructors
    public ParseException(
      string message
      ) : this(message, -1)
    {}

    public ParseException(
      string message,
      long position
      ) : base(message)
    {this.position = position;}

    public ParseException(
      Exception cause
      ) : this(null, cause)
    {}

    public ParseException(
      string message,
      Exception cause
      ) : this(message, cause, -1)
    {}

    public ParseException(
      string message,
      Exception cause,
      long position
      ) : base(message, cause)
    {this.position = position;}
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets the offset where error happened.</summary>
    */
    public long Position
    {
      get
      {return position;}
    }
    #endregion
    #endregion
    #endregion
  }
}