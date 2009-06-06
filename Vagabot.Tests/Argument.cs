using System;

namespace Binboo.Tests
{
	class Argument<T>
	{
		private bool _present;
		private readonly T _value;

		internal Argument(T value, bool present)
		{
			_value = value;
			_present = present;
		}

		public T Value
		{
			get { return _value; }
		}

		public bool IsPresent
		{
			get { return _present; }
		}

		public static implicit operator Argument<T>(T value)
		{
			return new Argument<T>(value, true);
		}
	}

	class Arguments
	{
		public static Argument<T> Missing<T>(T value)
		{
			return new Argument<T>(value, false);
		}
	}

}
