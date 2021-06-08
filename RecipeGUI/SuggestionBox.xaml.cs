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
using RecipeGUI.Preferences_Window;

namespace RecipeGUI
{
	/// <summary>
	/// Interaction logic for SuggestionBox.xaml
	/// </summary>
	public partial class SuggestionBox : UserControl
	{
		public List<string> suggestionStrings { get; set; }
		public int targetIndex = 0;
		public PreferencesManager prefs;

		public bool SugestionStackIsEmpty
		{
			get { return SuggestionsStack.Children.Count == 0; }
		}

		public SuggestionBox()
		{
			InitializeComponent();
		}

		private void TextBox_KeyUp(object sender, KeyEventArgs e)
		{
			if (suggestionStrings == null || !prefs.doAutocomplete) return;
			if(e.Key == Key.Enter && !SugestionStackIsEmpty)
			{
				TextBlock target = (TextBlock)SuggestionsStack.Children[targetIndex];
				if (target == null) return;
				SuggestionTextField.Text = target.Text;
				SuggestionTextField.CaretIndex = target.Text.Length;
				CloseSuggestionBox();
				targetIndex = 0;
				return;
			}
			if(e.Key == Key.Up && !SugestionStackIsEmpty)
			{
				if (targetIndex == 0) return;
				ClearPreviousTarget();
				targetIndex--;
				SetTarget((TextBlock)SuggestionsStack.Children[targetIndex]);
				ScrollTotarget();
				return;
			}
			if(e.Key == Key.Down && !SugestionStackIsEmpty)
			{
				if (targetIndex == SuggestionsStack.Children.Count -1) return;
				ClearPreviousTarget();
				targetIndex++;
				SetTarget((TextBlock)SuggestionsStack.Children[targetIndex]);
				ScrollTotarget();
				return;
			}
			string query = SuggestionTextField.Text;
			
			SuggestionsStack.Children.Clear();
			SuggestionsStack.Height = 100;
			SuggestionScroller.ScrollToHome();

			if (query.Length == 0)
			{
				CloseSuggestionBox();
			}
			else
			{
				OpenSuggestionBox();
			}

			int foundSuggestions = 0;
			foreach (string suggestion in suggestionStrings)
			{
				if (suggestion.ToLower().StartsWith(query.ToLower())){
					addItem(suggestion);
					foundSuggestions++;
				}
			}
			if (!SugestionStackIsEmpty)
			{
				TextBlock tb = (TextBlock)SuggestionsStack.Children[0];
				SuggestionsStack.Height = SuggestionsStack.Children.Count * tb.Height;
				SetTarget((TextBlock)SuggestionsStack.Children[0]);
				ScalePopup();
			}
			targetIndex = 0;
		}

		private void ScrollTotarget()
		{
			var target = SuggestionsStack.Children[targetIndex];
			var point = target.TranslatePoint(new Point(), SuggestionsStack);
			SuggestionScroller.ScrollToVerticalOffset(point.Y);
		}

		private void SetTarget(TextBlock newTarget)
		{
			newTarget.Background = Brushes.LightBlue;
		}
		private void ClearPreviousTarget()
		{
			TextBlock oldTarget = (TextBlock)SuggestionsStack.Children[targetIndex];
			if (oldTarget != null)
			{
				oldTarget.Background = Brushes.Transparent;
			}
		}

		private void addItem(string text)
		{
			TextBlock block = new TextBlock();

			// Add the text   
			block.Text = text;

			// A little style...   
			block.Margin = new Thickness(2, 3, 2, 3);
			block.Cursor = Cursors.Hand;

			// Mouse events   
			block.MouseLeftButtonUp += (sender, e) =>
			{
				SuggestionTextField.Text = (sender as TextBlock).Text;
				CloseSuggestionBox();
			};

			block.MouseEnter += (sender, e) =>
			{
				TextBlock b = sender as TextBlock;
				ClearPreviousTarget();
				SetTarget(b);
				targetIndex = SuggestionsStack.Children.IndexOf(b);
			};

			// Add to the panel   
			SuggestionsStack.Children.Add(block);
		}

		private void OpenSuggestionBox()
		{
			SuggestionPopup.IsOpen = true;
		}

		private void CloseSuggestionBox()
		{
			SuggestionPopup.IsOpen = false;
		}

		private void SuggestionTextField_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (SuggestionsStack.IsMouseOver)
			{
				return;
			}
			CloseSuggestionBox();
		}

		private void ScalePopup()
		{
			double width = SuggestionTextField.Width;
			var children = SuggestionsStack.Children;
			foreach(UIElement child in children)
			{
				TextBlock tb = (TextBlock)child;
				double difference = tb.Width.CompareTo(width);
				if (difference > 0) width = tb.Width;
			}

			SuggestionScroller.Width = width;
		}
	}
}
