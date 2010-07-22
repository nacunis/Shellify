/*
    Shellify, .NET implementation of Shell Link (.LNK) Binary File Format
    Copyright (C) 2010 Sebastien LEBRETON

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Runtime.InteropServices;
using Shellify.ExtraData;

namespace Shellify.IO
{
    public abstract class ExtraDataBlockHandler : IBinaryReadable, IBinaryWriteable
	{
		public abstract void ReadFrom(System.IO.BinaryReader reader);
		public abstract void WriteTo(System.IO.BinaryWriter writer);
	}

    public abstract class ExtraDataBlockHandler<T> : ExtraDataBlockHandler, ISizeComputable where T : ExtraDataBlock
	{
		
		protected T Item { get; set; }
		protected int BlockSize { get; set; }
        protected ShellLinkFile Context { get; set; }

		public virtual int ComputedSize
		{
			get
			{
				return Marshal.SizeOf(BlockSize) + Marshal.SizeOf(typeof(int));
			}
		}
		
		public ExtraDataBlockHandler(T item, ShellLinkFile context)
		{
			this.Item = item;
            this.Context = context;
		}
		
		public override void ReadFrom(System.IO.BinaryReader reader)
		{
			BlockSize = reader.ReadInt32();
			try
			{
				Item.Signature = (ExtraDataBlockSignature) (reader.ReadInt32());
			}
			catch (Exception)
			{
				Item.Signature = ExtraDataBlockSignature.Unknown;
			}
		}
		
		public override void WriteTo(System.IO.BinaryWriter writer)
		{
			writer.Write(ComputedSize);
			writer.Write((int) Item.Signature);
		}

        protected void CheckLength(int itemlength, int length, CheckLengthOption option) {
            switch(option) {
                case CheckLengthOption.MustBeEqual:
                    if (itemlength != length) {
                        throw new ArgumentException(string.Format("Incorrect length: {0}, must be exactly {1}", itemlength, length));
                    }
                    break;
                case CheckLengthOption.MustBeLower:
                    if (itemlength >= length) {
                        throw new ArgumentException(string.Format("Incorrect length: {0}, must be < {1}", itemlength, length));
                    }
                    break;
                case CheckLengthOption.MustBeHigher:
                    if (itemlength <= length) {
                        throw new ArgumentException(string.Format("Incorrect length: {0}, must be > {1}", itemlength, length));
                    }
                    break;
                case CheckLengthOption.MustBeLowerOrEqual:
                    if (itemlength > length)
                    {
                        throw new ArgumentException(string.Format("Incorrect length: {0}, must be <= {1}", itemlength, length));
                    }
                    break;
                case CheckLengthOption.MustBeHigherOrEqual:
                    if (itemlength < length)
                    {
                        throw new ArgumentException(string.Format("Incorrect length: {0}, must be >= {1}", itemlength, length));
                    }
                    break;
                default:
                    throw new ArgumentException(string.Format("Unknown option '{0}'", option));
            }
        }

        protected void CheckLength(Array item, int length, CheckLengthOption option) {
            CheckLength(item == null ? 0 : item.Length, length, option);
        }

        protected void CheckLength(string item, int length, CheckLengthOption option)
        {
            CheckLength(item == null ? 0 : item.Length, length, option);
        }
		
	}
}
