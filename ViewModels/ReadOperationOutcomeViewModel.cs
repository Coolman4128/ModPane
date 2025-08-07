using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ModPane.ViewModels;

public partial class ReadOperationOutcomeViewModel : ViewModelBase
{
    public ObservableCollection<UInt16> ReadValues { get; } = new ObservableCollection<UInt16>();
    public ObservableCollection<UInt16> RegisterNumbers { get; } = new ObservableCollection<UInt16>();
    private int startingAddress;
    private int count;


    [ObservableProperty]
    private ReadOperationViewModel _readOperation;

    public ReadOperationOutcomeViewModel(ReadOperationViewModel readOperation, int startingAddress, int count, List<UInt16> values)
    {
        _readOperation = readOperation;
        if (values == null || values.Count == 0)
        {
            throw new ArgumentException("Read values cannot be null or empty.", nameof(values));
        }
        if (values.Count != count)
        {
            throw new ArgumentException($"Expected {count} values, but received {values.Count}.", nameof(values));
        }
        this.startingAddress = startingAddress;
        this.count = count;
        for (int i = 0; i < count; i++)
        {
            ReadValues.Add(values[i]);
            RegisterNumbers.Add((UInt16)(startingAddress + i));
        }
    }


}