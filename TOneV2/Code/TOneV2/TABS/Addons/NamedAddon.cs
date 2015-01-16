using System;

namespace TABS.Addons
{
	public class NamedAddon : Attribute
	{
		protected string _Name;
		protected string _Description;

		public string Name { get { return _Name; } set { _Name = value; } }
		public string Description { get { return _Description; } set { _Description = value; } }

		public NamedAddon(string name)
		{
			_Name = name;
		}
		
		public NamedAddon(string name, string description) : this(name)
		{
			_Description = description;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
