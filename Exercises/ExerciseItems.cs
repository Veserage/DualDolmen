using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace DualDolmen
{
	public class LevelData
	{
		public required List<Exercise> Exercises { get; set; } // Коллекция упражнений для уровня
		public int CompletedWordsCount { get; set; } // Количество основных слов для изучения, которые уровень предоставляет
	}

	public class Exercise
	{
		public required string Type { get; set; } // Тип упражнения

		[JsonPropertyName("Content")]
		public JsonElement Content { get; set; } // динамический, т.к. разный тип контента
	}

	public class CompletedWord
	{
		public required string Content { get; set; } // Собственно, само слово
		public required string SpeechPart { get; set; } // Часть речи этого слова
	}

	// Для Cards
	public class CardPair
	{
		public required string Term { get; set; } // Слово на английском
		public required string Match { get; set; } // Перевод на русском
	}

}
