using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using System.IO;    

namespace ERP.ComClass
{
    internal class Chart
    {
        private const int CANVAS_SIZE = 400;
        private const int PIE_DIAMETER = 200;

        /// <summary>
        /// 生成饼图
        /// </summary>
        public Image CreatePieChart(string title, string subTitle, int width, int height,
                                     DataTable dt, string valueColumn, string nameColumn)
        {
            // 参数检查
            if (dt == null || dt.Rows.Count == 0) return null;
            if (string.IsNullOrEmpty(valueColumn)) return null;

            // 计算总和
            float sumData = 0;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[valueColumn] != DBNull.Value)
                    sumData += Convert.ToSingle(dr[valueColumn]);
            }
            if (sumData <= 0) return null;

            Bitmap bm = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bm);

            try
            {
                // 设置绘图质量
                g.ScaleTransform((float)width / CANVAS_SIZE, (float)height / CANVAS_SIZE);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.AntiAlias;

                // 背景和边框
                g.Clear(Color.White);
                g.DrawRectangle(Pens.Black, 0, 0, CANVAS_SIZE - 1, CANVAS_SIZE - 1);

                // 标题
                using (Font titleFont = new Font("Tahoma", 14, FontStyle.Bold))
                using (Font subTitleFont = new Font("Tahoma", 10))
                {
                    g.DrawString(title, titleFont, Brushes.Black, new PointF(5, 5));
                    g.DrawString(subTitle, subTitleFont, Brushes.Black, new PointF(7, 35));
                }

                // 画饼图
                float totalAngle = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    float value = Convert.ToSingle(dt.Rows[i][valueColumn]);
                    float curAngle = value / sumData * 360;
                    Color color = GetChartItemColor(i, dt.Rows.Count);

                    using (SolidBrush brush = new SolidBrush(color))
                    {
                        g.FillPie(brush, 100, 65, PIE_DIAMETER, PIE_DIAMETER, totalAngle, curAngle);
                    }
                    g.DrawPie(Pens.Black, 100, 65, PIE_DIAMETER, PIE_DIAMETER, totalAngle, curAngle);
                    totalAngle += curAngle;
                }

                // 图例边框
                g.DrawRectangle(Pens.Black, 200, 300, 199, 99);
                using (Font legendTitleFont = new Font("Tahoma", 12, FontStyle.Bold))
                {
                    g.DrawString("图表说明", legendTitleFont, Brushes.Black, new PointF(200, 300));
                }

                PointF boxOrigin = new PointF(210, 330);
                PointF textOrigin = new PointF(235, 326);

                // 图表说明
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    float value = Convert.ToSingle(dt.Rows[i][valueColumn]);
                    float percent = value / sumData * 100;
                    Color color = GetChartItemColor(i, dt.Rows.Count);

                    using (SolidBrush brush = new SolidBrush(color))
                    {
                        g.FillRectangle(brush, boxOrigin.X, boxOrigin.Y, 20, 10);
                    }
                    g.DrawRectangle(Pens.Black, boxOrigin.X, boxOrigin.Y, 20, 10);

                    string name = dt.Rows[i][nameColumn]?.ToString() ?? "";
                    string text = $"{name} ({percent:0}%)";

                    using (Font legendFont = new Font("Tahoma", 10))
                    {
                        g.DrawString(text, legendFont, Brushes.Black, textOrigin);
                    }

                    boxOrigin.Y += 15;
                    textOrigin.Y += 15;
                }

                return bm;
            }
            finally
            {
                g.Dispose();
            }
        }

        /// <summary>
        /// 根据索引自动生成颜色
        /// </summary>
        private Color GetChartItemColor(int index, int total)
        {
            if (total <= 0) return Color.Green;

            float hue = (float)index / total * 360f;
            return ColorFromHSV(hue, 0.7f, 0.9f);
        }

        /// <summary>
        /// HSV 转 RGB
        /// </summary>
        private Color ColorFromHSV(float hue, float saturation, float value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            float f = hue / 60 - (float)Math.Floor(hue / 60);

            value *= 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            switch (hi)
            {
                case 0: return Color.FromArgb(255, v, t, p);
                case 1: return Color.FromArgb(255, q, v, p);
                case 2: return Color.FromArgb(255, p, v, t);
                case 3: return Color.FromArgb(255, p, q, v);
                case 4: return Color.FromArgb(255, t, p, v);
                default: return Color.FromArgb(255, v, p, q);
            }
        }
    }
}
