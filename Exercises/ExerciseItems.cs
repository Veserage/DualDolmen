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
		public List<Exercise> Exercises { get; set; } // Коллекция упражнений для уровня
		public List<CompletedWord> CompletedWords { get; set; } // Основные слова для изучения, которые уровень предоставляет (добавляется в "Изученные слова")
	}

	public class Exercise
	{
		public string Type { get; set; } // Тип упражнения

		[JsonPropertyName("Content")]
		public JsonElement Content { get; set; } // динамический, т.к. разный тип контента
	}

	public class CompletedWord
	{
		public string Content { get; set; } // Собственно, само слово
		public string SpeechPart { get; set; } // Часть речи этого слова
	}

	// Для Cards
	public class CardPair
	{
		public string Term { get; set; } // Слово на английском
		public string Match { get; set; } // Перевод на русском
	}

	// Для InsertWord
	public class InsertWordContent
	{
		public string Text { get; set; }
		public List<string> Options { get; set; }
		public string Answer { get; set; }
	}

}
