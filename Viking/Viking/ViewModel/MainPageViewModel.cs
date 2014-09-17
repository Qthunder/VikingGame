namespace Viking.ViewModel
{
    using Viking.Common;
    public class MainPageViewModel :BasePropertyChanged
    {
        private BoardViewModel _boardViewModel;

        public BoardViewModel BoardViewModel 
        {
            get { return _boardViewModel; }

            set
            {
                _boardViewModel = value;
                RaisePropertyChanged(() => BoardViewModel);
            }
        }

        public MainPageViewModel()
        {
            BoardViewModel = new BoardViewModel();
        }
    }
}
