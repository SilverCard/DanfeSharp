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

using System;
using System.Collections.Generic;
using System.Text;

namespace org.pdfclown.util.metadata
{
  /**
    <summary>Version utility.</summary>
  */
  public static class VersionUtils
  {
    #region static
    #region interface
    #region public
    public static int CompareTo(
      IVersion version1,
      IVersion version2
      )
    {
      int comparison = 0;
      {
        IList<int> version1Numbers = version1.Numbers;
        IList<int> version2Numbers = version2.Numbers;
        for(
          int index = 0,
            length = Math.Min(version1Numbers.Count, version2Numbers.Count);
          index < length;
          index++
          )
        {
          comparison = version1Numbers[index] - version2Numbers[index];
          if(comparison != 0)
            break;
        }
        if(comparison == 0)
        {comparison = version1Numbers.Count - version2Numbers.Count;}
      }
      return Math.Sign(comparison);
    }

    public static string ToString(
      IVersion version
      )
    {
      StringBuilder versionStringBuilder = new StringBuilder();
      foreach(int number in version.Numbers)
      {
        if(versionStringBuilder.Length > 0)
        {versionStringBuilder.Append('.');}
        versionStringBuilder.Append(number);
      }
      return versionStringBuilder.ToString();
    }
    #endregion
    #endregion
    #endregion
  }
}