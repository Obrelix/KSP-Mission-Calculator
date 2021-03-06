﻿using Mission_Calculator.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace Mission_Calculator.Classes
{
    public class Routes
    {
        #region "General Declaration"

        SelestialObject _objIn, _objOut;        
        double _rotations;

        public string Name { get; set; }
        public SelestialObject ObjectFrom { get; set; }
        public SelestialObject ObjectTo { get; set; }
        public SelestialObject ObjectInner
        {
            get {return _objIn; }
            private set { _objIn = value; }
        }
        public SelestialObject ObjectOuter
        {
            get { return _objOut; }
            private set { _objOut = value; }
        }
        public Orbit OrbitFrom { get; set; }
        public Orbit OrbitTo { get; set; }
        public Brush TitleBrush { get; set; }
        public Brush ValueBrush { get; set; }
        public double DeparturePhaseAngle { get; private set; }
        public double EjectionAngle { get; private set; }
        public double EjectionVelocity { get; private set; }
        public double HohmannDV { get; private set; }
        public double DeltaVBugdet { get; private set; }
        public double TranferTime { get; private set; }
        public double IntervalBetweenLanchWindows { get; private set; }
        public double Rotations
        {
            get { return _rotations; }
            private set { _rotations = value; }
        }
        public string strTransferTime { get { return Globals.FormatTimeFromSecs(TranferTime); } }
        public string strInterval { get { return Globals.FormatTimeFromSecs(IntervalBetweenLanchWindows); } }
        public string strDv { get { return string.Format("{0:n0}", DeltaVBugdet) + " m/s"; } }
        public string strPhAngle { get { return DeparturePhaseAngle.ToString("n2") + " °"; } }
        public string strEjectionAngle { get { return EjectionAngle.ToString("n2") + " °"; } }
        public string strEjectionVelocity { get { return string.Format("{0:n0}", EjectionVelocity) + " m/s"; } }
        public string strHohmannDV { get { return string.Format("{0:n0}", HohmannDV) + " m/s"; } }

        #endregion

        #region "Constractor"

        public Routes(string Name, PlanetInfo PIFrom, PlanetInfo PITo, Brush ValueBrush)
        {
            this.Name = Name;
            this.ValueBrush = ValueBrush;
            TitleBrush = PITo.exp.Foreground;
            ObjectFrom = PIFrom.obj;
            ObjectTo = PITo.obj;
            OrbitFrom = PIFrom.orbit;
            OrbitTo = PITo.orbit;
            Update();
        }

        #endregion

        #region "Methods"

        public void Update()
        {
            try
            {
                IntervalBetweenLanchWindows = SMath.IntervalBetweenLanchWindows(this);
                TranferTime = SMath.CNHohmanTransferTime(this);
                TranferTime = SMath.HohmanTransferTime(this);
                DeparturePhaseAngle = SMath.CNDeparturePhaseAngle(this, out _rotations);
                DeparturePhaseAngle = SMath.DeparturePhaseAngle(this, out _rotations);
                HohmannDV = SMath.HohmannTransferDV(this);
                DeltaVBugdet = SMath.DeltaVCost(this);
                EjectionVelocity = SMath.EjectionVelocity(this);
                EjectionAngle = SMath.EjectionAngle(this);
                SMath.findActualObject(out _objIn, out _objOut);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<Run> ToShortRunList()
        {
            try
            {
                List<Run> objPropsRunList = new List<Run>();
                objPropsRunList.Clear();
                //objPropsRunList.Add(new Run("\t"));
                objPropsRunList.Add(Globals.coloredRun("[ ", ValueBrush));
                objPropsRunList.Add(Globals.coloredRun(ObjectFrom.Name, ObjectFrom.objectColour, "From "+ OrbitFrom.ToString()));
                objPropsRunList.Add(Globals.coloredRun(" --> ", TitleBrush));
                objPropsRunList.Add(Globals.coloredRun(ObjectTo.Name, ObjectTo.objectColour, "To " + OrbitTo.ToString()));
                objPropsRunList.Add(Globals.coloredRun(" ]", ValueBrush));
                objPropsRunList.Add(new Run(Environment.NewLine));
                objPropsRunList.Add(Globals.coloredRun("Ej. Angle: ", TitleBrush, "Ejection Angle."));
                objPropsRunList.Add(Globals.coloredRun(strEjectionAngle, ValueBrush, "Ejection Angle."));
                objPropsRunList.Add(new Run(Environment.NewLine));
                objPropsRunList.Add(Globals.coloredRun("Ej. ΔV.  : ", TitleBrush, "Ejection Velocity."));
                objPropsRunList.Add(Globals.coloredRun(strHohmannDV, ValueBrush, "Ejection Velocity."));
                objPropsRunList.Add(new Run(Environment.NewLine));
                objPropsRunList.Add(Globals.coloredRun("Ph. Angle: ", TitleBrush, "Departure Phase Angle."));
                objPropsRunList.Add(Globals.coloredRun(strPhAngle, ValueBrush, "Departure Phase Angle."));
                objPropsRunList.Add(new Run(Environment.NewLine));
                objPropsRunList.Add(Globals.coloredRun("Tr. ΔV   : ", TitleBrush, "Transfer ΔV\r\n(Calculated as summarise of routes from ΔV maps)"));
                objPropsRunList.Add(Globals.coloredRun(strDv, ValueBrush, "Transfer ΔV\r\n(Calculated as summarise of routes from ΔV maps)"));
                objPropsRunList.Add(new Run(Environment.NewLine));
                objPropsRunList.Add(Globals.coloredRun("Tr. Time : ", TitleBrush, "Hohmann's Transfer Time"));
                objPropsRunList.Add(Globals.coloredRun(strTransferTime, ValueBrush, "Hohmann's Transfer Time"));
                objPropsRunList.Add(new Run(Environment.NewLine));
                objPropsRunList.Add(Globals.coloredRun("I.B.L.W  : ", TitleBrush, "Interval between launch windows."));
                objPropsRunList.Add(Globals.coloredRun(strInterval, ValueBrush, "Interval between launch windows."));
                return objPropsRunList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public override string ToString()
        {
            try
            {
                string strPropsToString = string.Empty;
                strPropsToString += String.Format("{0,11} {1}\n", "Name:", Name);
                strPropsToString += String.Format("{0,11} {1,6} ( {2} )\n", "From:", ObjectFrom.Name, OrbitFrom.ToString());
                strPropsToString += String.Format("{0,11} {1,6} ( {2} )\n", "To:", ObjectTo.Name, OrbitFrom.ToString());
                strPropsToString += String.Format("{0,11} {1,6} \n", "Ph. Angle:", strPhAngle);
                strPropsToString += String.Format("{0,11} {1,6} \n", "Tr. Time:", strTransferTime);
                strPropsToString += String.Format("{0,11} {1,6} \n", "I.B.L.W:", strInterval);
                strPropsToString += String.Format("{0,11} {1,6} \n", "Δv:", strInterval);
                return strPropsToString;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

    }
}
