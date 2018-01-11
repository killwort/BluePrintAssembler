using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BluePrintAssembler.Domain;
using BluePrintAssembler.Utils;

namespace BluePrintAssembler.UI.VM
{
    public class Recipe:DraggableElement,ISelectableElement
    {
        private Domain.Recipe _myRecipe;
        private NotifyTaskCompletion<Bitmap> _icon;

        public Domain.Recipe MyRecipe
        {
            get => _myRecipe;
            set
            {
                if (Equals(value, _myRecipe)) return;
                _myRecipe = value;
                _icon = null;
                Sources = _myRecipe.Sources?.Select(x => new RecipeIO(x.Value)).ToArray();
                Results = _myRecipe.Results?.Select(x => new RecipeIO(x.Value)).ToArray();
                OnPropertyChanged();
                OnPropertyChanged(nameof(Sources));
                OnPropertyChanged(nameof(Results));
            }
        }

        public NotifyTaskCompletion<Bitmap> Icon => _icon ?? (_icon = new NotifyTaskCompletion<Bitmap>(Configuration.Instance.GetIcon(_myRecipe)));
        public RecipeIO[] Sources { get; private set; }
        public RecipeIO[] Results { get; private set; }
    }
}