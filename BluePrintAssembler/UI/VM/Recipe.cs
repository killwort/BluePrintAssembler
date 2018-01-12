﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BluePrintAssembler.Annotations;
using BluePrintAssembler.Domain;
using BluePrintAssembler.Utils;

namespace BluePrintAssembler.UI.VM
{
    public interface IGraphNode{}

    public interface IGraphEdge
    {
        IGraphNode Node1 { get; }
        IGraphNode Node2 { get; }
    }
    public class Recipe:ISelectableElement, INotifyPropertyChanged, IGraphNode
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
                Sources = _myRecipe.CurrentMode.Sources?.Select(x => new RecipeIO(this,x.Value)).ToArray()??new RecipeIO[0];
                Results = _myRecipe.CurrentMode.Results?.Select(x => new RecipeIO(this,x.Value)).ToArray() ?? new RecipeIO[0];
                OnPropertyChanged();
                OnPropertyChanged(nameof(Sources));
                OnPropertyChanged(nameof(Results));
            }
        }

        public NotifyTaskCompletion<Bitmap> Icon => _icon ?? (_icon = new NotifyTaskCompletion<Bitmap>(Configuration.Instance.GetIcon(_myRecipe)));
        public RecipeIO[] Sources { get; private set; }
        public RecipeIO[] Results { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}