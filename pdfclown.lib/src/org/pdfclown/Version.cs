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

using org.pdfclown.objects;
using org.pdfclown.util.metadata;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace org.pdfclown
{
  /**
    <summary>Generic PDF version number [PDF:1.6:H.1].</summary>
    <seealso cref="VersionEnum"/>
  */
  public sealed class Version
    : IVersion
  {
    #region static
    #region fields
    private static readonly Regex versionPattern = new Regex("^(\\d+)\\.(\\d+)$");
    private static readonly IDictionary<string,Version> versions = new Dictionary<string,Version>();
    #endregion

    #region interface
    #region public
    public static Version Get(
      PdfName version
      )
    {return Get(version.RawValue);}

    public static Version Get(
      string version
      )
    {
      if(!versions.ContainsKey(version))
      {
        Match versionMatch = versionPattern.Match(version);
        if(!versionMatch.Success)
          throw new Exception("Invalid PDF version format: '" + versionPattern + "' pattern expected.");

        Version versionObject = new Version(Int32.Parse(versionMatch.Groups[1].Value),Int32.Parse(versionMatch.Groups[2].Value));
        versions[version] = versionObject;
      }
      return versions[version];
    }
    #endregion
    #endregion
    #endregion

    #region dynamic
    #region fields
    private readonly int major;
    private readonly int minor;
    #endregion

    #region constructors
    private Version(
      int major,
      int minor
      )
    {
      this.major = major;
      this.minor = minor;
    }
    #endregion

    #region interface
    #region public
    public int Major
    {
      get
      {return major;}
    }

    public int Minor
    {
      get
      {return minor;}
    }

    public override string ToString(
      )
    {return VersionUtils.ToString(this);}

    #region IVersion
    public IList<int> Numbers
    {
      get
      {return new List<int>{major, minor};}
    }

    #region IComparable
    public int CompareTo(
      IVersion value
      )
    {return VersionUtils.CompareTo(this, value);}
    #endregion
    #endregion
    #endregion
    #endregion
    #endregion
  }
}