/*
  Copyright 2006-2011 Stefano Chizzolini. http://www.pdfclown.org

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

namespace org.pdfclown.tokens
{
  /**
    <summary>Cross-reference table entry [PDF:1.6:3.4.3].</summary>
  */
  public class XRefEntry
    : ICloneable
  {
    #region types
    /**
      <summary>Cross-reference table entry usage [PDF:1.6:3.4.3].</summary>
    */
    public enum UsageEnum
    {
      /**
        <summary>Free entry.</summary>
      */
      Free,
      /**
        <summary>Ordinary (uncompressed) object entry.</summary>
      */
      InUse,
      /**
        <summary>Compressed object entry [PDF:1.6:3.4.6].</summary>
      */
      InUseCompressed
    }
    #endregion

    #region static
    #region fields
    /**
      <summary>Unreusable generation [PDF:1.6:3.4.3].</summary>
    */
    public static readonly int GenerationUnreusable = 65535;

    /**
      <summary>Undefined offset.</summary>
    */
    public static readonly int UndefinedOffset = -1;
    #endregion
    #endregion

    #region dynamic
    #region fields
    private int number;
    private int generation;
    private int offset;
    private int streamNumber;
    private UsageEnum usage;
    #endregion

    #region constructors
    /**
      <summary>Instantiates a new in-use ordinary (uncompressed) object entry.</summary>
      <param name="number">Object number.</param>
      <param name="generation">Generation number.</param>
    */
    public XRefEntry(
      int number,
      int generation
      ) : this(number, generation, -1, UsageEnum.InUse)
    {}

    /**
      <summary>Instantiates an original ordinary (uncompressed) object entry.</summary>
      <param name="number">Object number.</param>
      <param name="generation">Generation number.</param>
      <param name="offset">Indirect-object byte offset within the serialized file (in-use entry),
        or the next free-object object number (free entry).</param>
      <param name="usage">Usage state.</param>
    */
    public XRefEntry(
      int number,
      int generation,
      int offset,
      UsageEnum usage
      ) : this(number, generation, offset, usage, -1)
    {}

    /**
      <summary>Instantiates a compressed object entry.</summary>
      <param name="number">Object number.</param>
      <param name="offset">Object index within its object stream.</param>
      <param name="streamNumber">Object number of the object stream in which this object is stored.
      </param>
    */
    public XRefEntry(
      int number,
      int offset,
      int streamNumber
      ) : this(number, 0, offset, UsageEnum.InUseCompressed, streamNumber)
    {}

    private XRefEntry(
      int number,
      int generation,
      int offset,
      UsageEnum usage,
      int streamNumber
      )
    {
      this.number = number;
      this.generation = generation;
      this.offset = offset;
      this.usage = usage;
      this.streamNumber = streamNumber;
    }
    #endregion

    #region interface
    #region public
    /**
      <summary>Gets the generation number.</summary>
    */
    public int Generation
    {
      get
      {return generation;}
      internal set
      {generation = value;}
    }

    /**
      <summary>Gets the object number.</summary>
    */
    public int Number
    {
      get
      {return number;}
      internal set
      {number = value;}
    }

    /**
      <summary>Gets its indirect-object byte offset within the serialized file (in-use entry),
      the next free-object object number (free entry) or the object index within its object stream
      (compressed entry).</summary>
    */
    public int Offset
    {
      get
      {return offset;}
      internal set
      {offset = value;}
    }

    /**
      <summary>Gets the object number of the object stream in which this object is stored [PDF:1.6:3.4.7],
      in case it is a <see cref="UsageEnum.InUseCompressed">compressed</see> one.</summary>
      <returns>-1 in case this is <see cref="UsageEnum.InUse">not a compressed</see>-object entry.</returns>
    */
    public int StreamNumber
    {
      get
      {return streamNumber;}
      internal set
      {streamNumber = value;}
    }

    /**
      <summary>Gets the usage state.</summary>
    */
    public UsageEnum Usage
    {
      get
      {return usage;}
      internal set
      {usage = value;}
    }

    #region ICloneable
    public object Clone(
      )
    {return MemberwiseClone();}
    #endregion
    #endregion
    #endregion
    #endregion
  }
}