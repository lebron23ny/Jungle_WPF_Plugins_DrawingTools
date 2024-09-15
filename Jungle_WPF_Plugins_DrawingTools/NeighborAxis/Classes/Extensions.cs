using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSM = Tekla.Structures.Model;
using TS = Tekla.Structures;

namespace Jungle_WPF_Plugins_DrawingTools.NeighborAxis.Classes
{
    public class Extensions
    {
        internal static TSM.ModelObject SelectModelObject(int objectId)
        {
            TSM.ModelObject result = null;
            TSM.ModelObject modelObject =
                new TSM.Model().SelectModelObject(new TS.Identifier(objectId));
            result = modelObject;
            return result;
        }

        internal static void FindNextAndPreviousLabelText(string curLabel, string[] labelsX, string[] labelsY, out string previous, out string next)
        {
            previous = null;
            next = null;

            if (labelsX.Contains(curLabel))
            {
                try
                {
                    int labelIndex = Array.FindIndex(labelsX, l => l.Contains(curLabel));
                    int countLabelsX = labelsX.Length;
                    if(labelIndex==0)
                    {
                        next = labelsX[1];
                        previous = null;
                    }
                    else if(labelIndex + 1==countLabelsX)
                    {
                        next = null;
                        previous = labelsX[labelIndex - 1];
                    }
                    else
                    {
                        next = labelsX[labelIndex + 1];
                        previous = labelsX[labelIndex - 1];
                    }
                    
                }
                catch { }
            }
            else if (labelsY.Contains(curLabel))
            {
                try
                {
                    int labelIndex = Array.FindIndex(labelsY, l => l.Contains(curLabel));
                    int countLabelsY = labelsX.Length;
                    if (labelIndex == 0)
                    {
                        next = labelsY[1];
                        previous = null;
                    }
                    else if (labelIndex + 1 == countLabelsY)
                    {
                        next = null;
                        previous = labelsY[labelIndex - 1];
                    }
                    else
                    {
                        next = labelsY[labelIndex + 1];
                        previous = labelsY[labelIndex - 1];
                    }


                    next = labelsY[labelIndex + 1];
                    previous = labelsY[labelIndex - 1];
                }
                catch { }
            }
        }
    }
}
