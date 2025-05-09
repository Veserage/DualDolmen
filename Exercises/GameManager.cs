using DualDolmen.Exercises.Pages;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DualDolmen.Exercises
{
	public class GameManager
	{
		private List<Exercise> exercises;
		private int currentIndex = 0;

		public GameManager(int levelNumber)
		{
			
			var json = File.ReadAllText($"Levels/Level{levelNumber}.json");
			var levelData = JsonSerializer.Deserialize<LevelData>(json);
			exercises = levelData.Exercises;
		}

		public Page GetCurrentExercisePage()
		{
			var exercise = exercises[currentIndex];
			return exercise.Type switch
			{
				"Cards" => new CardsPage(exercise, this),
				"OddOne" => new OddOnePage(exercise, this),
				"InsertWord" => new InsertWordPage(exercise, this),
				_ => throw new Exception($"Неизвестный тип упражнения: {exercise.Type}")
			};
		}

		public void AdvanceExercise() => currentIndex++;

		public bool HasMoreExercises => currentIndex < exercises.Count;

		public void RegisterFailure() { /* TODO: сохраняешь ошибку */ }

		public void MarkLevelAsCompleted() { /* TODO: помечаешь уровень завершённым */ }
	}
	// TODO: Нужно сохранение прогресса, и отображение статы в ЛК, еще пройденные уровни красить зеленым в меню выбора уровней
}
