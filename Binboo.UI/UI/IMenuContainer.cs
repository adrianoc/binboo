using System;

namespace Binboo.Core.UI
{
	public interface IMenuContainer
	{
		void Add(string desc, Action action);
	}
}
