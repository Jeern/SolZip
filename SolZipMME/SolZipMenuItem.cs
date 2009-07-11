using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagedMenuAddInViews;
using System.Text.RegularExpressions;

namespace SolZipMME
{
    public class SolZipMenuItem : MenuItemView
    {
        private string m_Caption = "";
        private Guid m_Id = Guid.NewGuid();

        public SolZipMenuItem(string caption)
        {
            m_Caption = caption;
        }

        public override string Caption
        {
            get { return m_Caption; }
        }

        public override Guid Id
        {
            get { return m_Id; }
        }

        public override bool Seperator
        {
            get { return false; }
        }

        public override MenuItemView Parent
        {
            get { return null; }
        }

        public override Regex VisibleWhenCompliantName
        {
            get 
            { 
                return new Regex(@"\.csproj");
            }
        }
    }
}
