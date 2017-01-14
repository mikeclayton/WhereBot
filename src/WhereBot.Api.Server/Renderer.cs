using System.Drawing;
using WhereBot.Api.Data.Domain;

namespace WhereBot.Api
{

    public static class Renderer
    {

        public static void AddResource(Graphics graphics, Resource resource, Brush fill, Pen border, Font font, Brush fontColor)
        {
            var location = resource.Location;
            // draw the label
            //var label = new Rectangle(location.X + 20, location.Y, 100, 20);
            //g.FillRectangle(Brushes.White, label);
            //g.DrawRectangle(Pens.Black, label);
            // draw the dot
            graphics.FillEllipse(fill, location.X - 10, location.Y - 10, 20, 20);
            graphics.DrawEllipse(border, location.X - 10, location.Y - 10, 20, 20);
        }

        public static void AddLocation(Graphics graphics, Location location, Brush fill, Pen border, Font font, Brush fontColor)
        {
            // draw the label
            var label = new Rectangle(location.X - 2, location.Y - 2, 30, 20);
            graphics.FillRectangle(fill, label);
            graphics.DrawRectangle(border, label);
            graphics.DrawString(location.Name, font, fontColor, location.X, location.Y);
        }

    }

}
