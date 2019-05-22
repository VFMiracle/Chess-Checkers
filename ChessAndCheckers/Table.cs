using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ChessAndCheckers
{
    public class Table
    {
        private static Rectangle warningSection;
        private static Brush wrnSectionOrgColor;
        private static Rectangle calloutSection;
        private static Brush calloutOrgColor;
        private static IEnumerable<Rectangle> tabSections;

        public static void SetTabSections(DependencyObject d)
        {
            tabSections = FindAllTableSections(d);
        }

        public static void HighLightSections(List<Vector> poss)
        {
            if (poss != null)
            {
                foreach (Rectangle section in tabSections)
                    for (int i = 0; i < poss.Count; i++)
                        if (section.Margin.Left == poss.ElementAt(i).X && section.Margin.Top == poss.ElementAt(i).Y)
                            section.Fill = new SolidColorBrush(Colors.Aqua);
            }
        }

        public static void ResetTable()
        {
            foreach(Rectangle section in tabSections)
            {
                char column = section.Name.ElementAt(0);
                int row = section.Name.ElementAt(1);
                if(column == 'A' || column == 'C' || column == 'E' || column == 'G')
                {
                    if (row % 2 == 0) section.Fill = new SolidColorBrush(Colors.White);
                    else section.Fill = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    if (row % 2 == 0) section.Fill = new SolidColorBrush(Colors.Black);
                    else section.Fill = new SolidColorBrush(Colors.White);
                }
            }
        }

        public static void MarkWarningSection(Vector pos)
        {
            foreach(Rectangle section in tabSections)
            {
                if (section.Margin.Left == pos.X && section.Margin.Top == pos.Y)
                {
                    if (warningSection == null)
                    {
                        wrnSectionOrgColor = section.Fill;
                        section.Fill = new SolidColorBrush(Colors.IndianRed);
                        warningSection = section;
                    }
                }
            }
        }

        public static void ResetWarningSection()
        {
            if(warningSection != null) warningSection.Fill = wrnSectionOrgColor;
            wrnSectionOrgColor = null;
            warningSection = null;
        }

        public static void CalloutSection(Vector pos)
        {
            foreach (Rectangle section in tabSections)
            {
                if (section.Margin.Left == pos.X && section.Margin.Top == pos.Y)
                {
                    calloutOrgColor = section.Fill;
                    section.Fill = new SolidColorBrush(Colors.Yellow);
                    calloutSection = section;
                }
            }
        }

        public static void ResetCalloutSection()
        {
            if(calloutSection != null) calloutSection.Fill = calloutOrgColor;
            calloutOrgColor = null;
            calloutSection = null;
        }

        private static IEnumerable<Rectangle> FindAllTableSections(DependencyObject depObj)
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is Rectangle)
                    {
                        yield return (Rectangle)child;
                    }

                    foreach (Rectangle childOfChild in FindAllTableSections(child))
                    {
                        if ((string)childOfChild.Tag != "SlctSqr")
                            yield return childOfChild;
                    }
                }
            }
        }
    }
}
