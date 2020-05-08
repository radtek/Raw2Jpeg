using System;
using System.Collections.Generic;
using System.Text;

namespace Raw2Jpeg.Helper
{
	public struct YCbCr
	{
		private float _y;
		private float _cb;
		private float _cr;

		public YCbCr(float y, float cb, float cr)
		{
			this._y = y;
			this._cb = cb;
			this._cr = cr;
		}

		public float Y
		{
			get { return this._y; }
			set { this._y = value; }
		}

		public float Cb
		{
			get { return this._cb; }
			set { this._cb = value; }
		}

		public float Cr
		{
			get { return this._cr; }
			set { this._cr = value; }
		}

		public bool Equals(YCbCr ycbcr)
		{
			return (this.Y == ycbcr.Y) && (this.Cb == ycbcr.Cb) && (this.Cr == ycbcr.Cr);
		}
	}
}
