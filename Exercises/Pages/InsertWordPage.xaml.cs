using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DualDolmen.Exercises.Pages
{
	/// <summary>
	/// Interaction logic for InsertWordPage.xaml
	/// </summary>
	public partial class InsertWordPage : Page
	{
		private GameManager gameManager;
		private Exercise exercise;

		private string Sentence;
		private List<string> Words;
		private string Answer;

		public InsertWordPage(Exercise exercise, GameManager gameManager)
		{
			InitializeComponent();
			this.exercise = exercise;
			this.gameManager = gameManager;

			SentenceTextBlock.Text = exercise.Content.GetProperty("Text").ToString();
			this.Answer = exercise.Content.GetProperty("Answer").ToString();

			InitWords();
		
		}

		private void InitWords()
		{
			Words = exercise.Content.GetProperty("Words").EnumerateArray().Select(x => x.GetString()).ToList();
			
			// Расположение кнопок со словами
			foreach (string word in Words)
			{
				var btn = new Button
				{
					Content = word,
					FontSize = 30,
					Padding = new Thickness(12, 8, 12, 8),
					Margin = new Thickness(20),
					Background = Brushes.Snow
				};

				btn.Click += Word_Click;
				WordsPanel.Children.Add(btn);
			}

			// Обновление номера упражнения в левом верхнем углу
			ProgressTextBlock.Text = $"{gameManager.currentExerciseIndex + 1}/{gameManager.exercises.Count}";
		}

		private async void Word_Click(object sender, RoutedEventArgs e)
		{
			var btn = sender as Button;
			if (btn == null) return;

			if (btn.Content.ToString() == Answer)
			{
				btn.Background = Brushes.LightGreen;
				await Task.Delay(1000);

                // Если упражнений больше не осталось
                if (!gameManager.HasMoreExercises)
                {
                    gameManager.MarkLevelAsCompleted();
                    NavigationService.Navigate(new MenuExercises(gameManager.currentUsername));
                    return;
                }

                gameManager.AdvanceExercise();
				NavigationService.Navigate(gameManager.GetCurrentExercisePage());
			}
			else
			{
				btn.Background = Brushes.IndianRed;
				await Task.Delay(1000);  
				btn.Background = Brushes.Snow;  
			}
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			NavigationService.Navigate(new MenuExercises(gameManager.currentUsername));
		}
	}
}
