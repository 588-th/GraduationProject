using Logic.Model;
using System.Windows.Controls;
using System.Windows.Input;

namespace Interface.Items
{
    public partial class ItemModel : UserControl
    {
        public event EventHandler ModelSelected;
        public event EventHandler ModelDeleted;
        private static SRModel _model;

        public ItemModel(SRModel model)
        {
            InitializeComponent();

            if (model != null)
            {
                _model = model;
                LoadData();
                MouseUp += ItemModel_MouseUp;
                ButtonModelDelete.Click += ButtonModelDelete_Click;
            }
        }

        public SRModel GetModel()
        {
            return _model;
        }

        private void LoadData()
        {
            TextBlockName.Text = _model.Name;
            TextBlockCreateDate.Text = _model.CreateDate;
            TextBlockNumStates.Text = _model.NumStates.ToString();
            TextBlockNumWords.Text = _model.NumWords.ToString();
            TextBlockTrainingTime.Text = _model.TrainingTime;
        }

        private void ButtonModelDelete_Click(object sender, EventArgs e)
        {
            ModelDeleted?.Invoke(this, EventArgs.Empty);
        }

        private void ItemModel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ModelSelected?.Invoke(this, EventArgs.Empty);
        }
    }
}
