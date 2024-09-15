using System;
using TSD = Tekla.Structures.Drawing;
using TSG = Tekla.Structures.Geometry3d;

namespace Jungle_WPF_Plugins_DrawingTools.FragmentSymbol.Extension
{
    public class Tools
    {
        /// <summary>
        /// Возвращает ширину объекта TSD.Text c текстом text,
        /// c настройками текста textAttributes
        /// </summary>
        /// <param name="text"></param>
        /// <param name="textAttributes"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        public static double GetWidthText(string text, TSD.Text.TextAttributes textAttributes, TSD.ViewBase view)
        {

            TSD.Text textObj = new TSD.Text(view, new TSG.Point(), text, textAttributes);
            textObj.Insert();
            double width = textObj.GetAxisAlignedBoundingBox().Width;
            textObj.Delete();
            return width;
        }


        /// <summary>
        /// Возвращает ширину объекта TSD.Text c текстом text,
        /// c настройками текста textAttributes
        /// </summary>
        /// <param name="text"></param>
        /// <param name="textAttributes"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        public static double GetHeightText(string text, TSD.Text.TextAttributes textAttributes, TSD.ViewBase view)
        {

            TSD.Text textObj = new TSD.Text(view, new TSG.Point(), text, textAttributes);
            textObj.Insert();
            double width = textObj.GetAxisAlignedBoundingBox().Height;
            textObj.Delete();
            return width;
        }


        public static double GetParseDouble(string text)
        {
            
            string[] a = text.Split(new char[] { ',', '.' });

            if (a.Length > 1)
            {
                string number_string = a[0] + "," + a[1];
                return Convert.ToDouble(number_string);
            }
            else
                return Convert.ToDouble(text);
        }
    }
}
