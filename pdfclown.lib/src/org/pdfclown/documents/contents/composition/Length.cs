/*
  Copyright 2010-2012 Stefano Chizzolini. http://www.pdfclown.org

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

namespace org.pdfclown.documents.contents.composition
{
  /**
    <summary>Distance measure.</summary>
  */
  public sealed class Length
  {
    /**
      <summary>Measurement mode.</summary>
    */
    public enum UnitModeEnum
    {
      /**
        <summary>Values are expressed as absolute measures.</summary>
      */
      Absolute,
      /**
        <summary>Values are expressed as ratios relative to a specified base value.</summary>
      */
      Relative
    }

    private UnitModeEnum unitMode;
    private double value;

    public Length(
      double value,
      UnitModeEnum unitMode
      )
    {
      this.value = value;
      this.unitMode = unitMode;
    }

    /**
      <summary>Gets the resolved distance value.</summary>
      <remarks>This method ensures that relative distance values are transformed according
      to the specified base value.</remarks>
      <param name="baseValue">Value used to resolve relative values.</param>
    */
    public double GetValue(
      double baseValue
      )
    {
      switch(unitMode)
      {
        case UnitModeEnum.Absolute:
          return value;
        case UnitModeEnum.Relative:
          return baseValue * value;
        default:
          throw new NotSupportedException(unitMode.GetType().Name + " not supported.");
      }
    }

    public override string ToString(
      )
    {return value + " (" + unitMode + ")";}

    /**
      <summary>Gets/Sets the measurement mode applied to the distance value.</summary>
    */
    public UnitModeEnum UnitMode
    {
      get
      {return unitMode;}
      set
      {unitMode = value;}
    }

    /**
      <summary>Gets/Sets the distance value.</summary>
      <remarks>According to the applied unit mode, this value can be
      either an absolute measure or a ratio to be resolved through a base value.</remarks>
    */
    public double Value
    {
      get
      {return value;}
      set
      {this.value = value;}
    }
  }
}