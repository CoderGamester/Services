using System;

namespace GameLovers.Services
{
	/// <summary>
	/// Contains all the data in the scope to generate and maintain the random generated values
	/// </summary>
	[Serializable]
	public struct RngData
	{
		public int Seed;
		public int Count;
		public int[] State;
	}

	/// <summary>
	/// Implement this interface if you use a data structure in a class to pass the values by reference
	/// and thus updating directly your internal data container
	/// </summary>
	public interface IRngData
	{
		/// <summary>
		/// The data container the state of the RNG information change.
		/// </summary>
		RngData Data { get; set; }
}

	/// <summary>
	/// This Service provides the necessary behaviour to manage the random generated values with always a deterministic result
	/// Based on the .Net library Random class <see cref="https://referencesource.microsoft.com/#mscorlib/system/random.cs"/>
	/// </summary>
	public interface IRngService
	{
		/// <summary>
		/// The <see cref="RngData"/> that this service is manipulating
		/// </summary>
		public RngData Data { get; }

		/// <summary>
		/// Returns the number of times the Rng has been counted;
		/// </summary>
		int Counter { get; }

		/// <summary>
		/// Requests the next <see cref="int"/> generated value without changing the state.
		/// Calling this multiple times in sequence gives always the same result.
		/// </summary>
		int Peek { get; }

		/// <summary>
		/// Requests the next <see cref="float"/> generated value without changing the state.
		/// Calling this multiple times in sequence gives always the same result.
		/// </summary>
		floatP Peekfloat { get; }

		/// <summary>
		/// Requests the next <see cref="int"/> generated value
		/// </summary>
		int Next { get; }

		/// <summary>
		/// Requests the next <see cref="floatP"/> generated value
		/// </summary>
		floatP Nextfloat { get; }

		/// <inheritdoc cref="Range(int,int,int[],bool)"/>
		/// <remarks>
		/// Calling this multiple times with the same parameters in sequence gives always the same result.
		/// </remarks>
		int PeekRange(int min, int max, bool maxInclusive = false);

		/// <inheritdoc cref="Range(floatP,floatP,int[],bool)"/>
		/// <remarks>
		/// Calling this multiple times with the same parameters in sequence gives always the same result.
		/// </remarks>
		floatP PeekRange(floatP min, floatP max, bool maxInclusive = true);

		/// <inheritdoc cref="Range(int,int,int[],bool)"/>
		int Range(int min, int max, bool maxInclusive = false);

		/// <inheritdoc cref="Range(floatP,floatP,int[],bool)"/>
		floatP Range(floatP min, floatP max, bool maxInclusive = true);

		/// <summary>
		/// Restores the current RNG state to the given <paramref name="count"/>.
		/// The value can be defined for a state in the past or a state in the future.
		/// </summary>
		void Restore(int count);
	}

	/// <inheritdoc />
	public class RngService : IRngService
	{
		private const int _basicSeed = 161803398;
		private const int _stateLength = 56;
		private const int _helperInc = 21;
		private const int _valueIndex = 0;

		private IRngData _rngData;

		/// <inheritdoc />
		public int Counter => Data.Count;

		/// <inheritdoc />
		public int Peek => PeekRange(0, int.MaxValue);

		/// <inheritdoc />
		public floatP Peekfloat => PeekRange((floatP) 0, floatP.MaxValue);

		/// <inheritdoc />
		public int Next => Range(0, int.MaxValue);

		/// <inheritdoc />
		public floatP Nextfloat => Range((floatP) 0, floatP.MaxValue);

		/// <inheritdoc />
		public RngData Data
		{
			get => _rngData.Data;
			private set => _rngData.Data = value;
		}

		public RngService(RngData rngData)
		{
			_rngData = new InternalRngData { Data = rngData };
		}

		public RngService(IRngData data)
		{
			_rngData = data;
		}

		/// <inheritdoc />
		public int PeekRange(int min, int max, bool maxInclusive = false)
		{
			return Range(min, max, CopyRngState(Data.State), maxInclusive);
		}

		/// <inheritdoc />
		public floatP PeekRange(floatP min, floatP max, bool maxInclusive = true)
		{
			return Range(min, max, CopyRngState(Data.State), maxInclusive);
		}

		/// <inheritdoc />
		public int Range(int min, int max, bool maxInclusive = false)
		{
			var data = Data;

			data.Count++;

			Data = data;

			return Range(min, max, Data.State, maxInclusive);
		}

		/// <inheritdoc />
		public floatP Range(floatP min, floatP max, bool maxInclusive = true)
		{
			var data = Data;

			data.Count++;

			Data = data;

			return Range(min, max, Data.State, maxInclusive);
		}

		/// <inheritdoc />
		public void Restore(int count)
		{
			var data = Data;

			data.Count = count;
			data.State = Restore(count, Data.Seed);

			Data = data;
		}

		/// <summary>
		/// Restores the current RNG state to the given <paramref name="count"/> based on the given <paramref name="seed"/>.
		/// The <paramref name="count "/> value can be defined for a state in the past or a state in the future.
		/// </summary>
		public static int[] Restore(int count, int seed)
		{
			var newState = GenerateRngState(seed);

			for (var i = 0; i < count; i++)
			{
				NextNumber(newState);
			}

			return newState;
		}

		/// <summary>
		/// Requests a random generated <see cref="int"/> value between the given <paramref name="min"/> and <paramref name="max"/>,
		/// without changing the state with  the max value inclusive depending on the given <paramref name="maxInclusive"/>
		/// </summary>
		public static int Range(int min, int max, int[] rndState, bool maxInclusive)
		{
			var floatMin = min;
			var floatMax = max;

			return Range(floatMin, floatMax, rndState, maxInclusive);
		}

		/// <summary>
		/// Requests a random generated <see cref="int"/> value between the given <paramref name="min"/> and <paramref name="max"/>,
		/// without changing the state with  the max value inclusive depending on the given <paramref name="maxInclusive"/>
		/// </summary>
		/// <remarks>
		/// This is not a deterministic result on the range request due to the flaoting point precision
		/// </remarks>
		public static floatP Range(floatP min, floatP max, int[] rndState, bool maxInclusive)
		{
			if (min > max || maxInclusive && Math.Abs(min - max) < floatP.Epsilon)
			{
				throw new IndexOutOfRangeException("The min range value must be less the max range value");
			}

			if (Math.Abs(min - max) < floatP.Epsilon)
			{
				return min;
			}

			var range = max - min;
			var value = NextNumber(rndState);

			value = maxInclusive && value == int.MaxValue ? value - 1 : value;

			return range * value / int.MaxValue + min;
		}

		/// <summary>
		/// Creates a new state as an exact copy of the given <paramref name="state"/>.
		/// Use this method if you want to generate a new random number without changing the RNG current state.
		/// </summary>
		/// <exception cref="IndexOutOfRangeException">
		/// Thrown if the given <paramref name="state"/> does not have the length equal to <seealso cref="_stateLength"/>
		/// </exception>
		public static int[] CopyRngState(int[] state)
		{
			if (state == null || state.Length != _stateLength)
			{
				throw new IndexOutOfRangeException($"The Random data created has the wrong state date." +
												   $"It should have a lenght of {_stateLength.ToString()} but has {state?.Length}");
			}

			var newState = new int[_stateLength];

			Array.Copy(state, newState, _stateLength);

			return newState;
		}

		/// <summary>
		/// Generates a completely new state rng state based on the given <paramref name="seed"/>.
		/// Based on the publish work of D.E. Knuth <see cref="https://www.informit.com/articles/article.aspx?p=2221790"/>
		/// </summary>
		public static int[] GenerateRngState(int seed)
		{
			var value = _basicSeed - (seed == int.MinValue ? int.MaxValue : Math.Abs(seed));
			var state = new int[_stateLength];

			state[_stateLength - 1] = value;
			state[_valueIndex] = 0;

			//Apparently the range [1..55] is special (Knuth)
			for (int i = 1, j = 1; i < _stateLength - 1; i++)
			{
				var index = (_helperInc * i) % (_stateLength - 1);

				state[index] = j;

				j = value - j;

				if (j < 0)
				{
					j += int.MaxValue;
				}

				value = state[index];
			}

			for (var k = 1; k < 5; k++)
			{
				for (var i = 1; i < _stateLength; i++)
				{
					state[i] -= state[1 + (i + 30) % (_stateLength - 1)];

					if (state[i] < 0)
					{
						state[i] += int.MaxValue;
					}
				}
			}

			return state;
		}

		/// <summary>
		/// Generates the next random number between [0...int.MaxValue] based on the given <paramref name="rndState"/>
		/// </summary>
		private static int NextNumber(int[] rndState)
		{
			var index1 = rndState[_valueIndex] + 1;
			var index2 = index1 + _helperInc + 1;

			index1 = index1 < _stateLength ? index1 : 1;
			index2 = index2 < _stateLength ? index2 : 1;

			var ret = rndState[index1] - rndState[index2];

			ret = ret < 0 ? ret + int.MaxValue : ret;

			rndState[index1] = ret;
			rndState[_valueIndex] = index1;

			return ret;
		}

		/// <summary>
		/// Used only in the scope of this service in case that no class is passed into the object
		/// </summary>
		private class InternalRngData : IRngData
		{
			/// <inheritdoc />
			public RngData Data { get; set; }
		}
	}
}
