using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tyd;
using System.Drawing;

namespace TyDSINC.Nodes
{
	class TydImage : TydNode
	{
        //Properties
        public string Value { get; set; }

        public TydImage(string name, string val, int docLine = -1) : base(name, docLine)
        {
            Value = val;
        }

        public TydImage(string name, string from, string dest, int width = 128, int height = 128, int docLine = -1) : base(name, docLine)
		{
            Image img = Image.FromFile(from);
            img = img.GetThumbnailImage(width, height, null, IntPtr.Zero);
            img.Save(dest, System.Drawing.Imaging.ImageFormat.Png);
            Value = dest;
		}

        public override TydNode DeepClone()
        {
            var c = new TydString(_name, Value, DocLine);
            c.DocIndexEnd = DocIndexEnd;
            return c;
        }

        public override string ToString()
        {
            return string.Format("{0}=\"{1}\"", Name ?? "NullName", Value);
        }

        public Image ToThumbnail(int width = 128, int height = 128)
		{
            Image img = Image.FromFile(Value);
            return img.GetThumbnailImage(width, height, null, IntPtr.Zero);
		}

        public Image ToImage()
		{
            return Image.FromFile(Value);
		}
    }
}
