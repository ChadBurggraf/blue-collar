/**
 * Form view for signaling Working models.
 */
var WorkingSignalFormView = FormView.extend({
    serializers: {
        "Id": new IntFieldSerializer()
    },

    template: _.template($('#working-signal-form-template').html()),

    validators: {
        "Id": [
            new RequiredValidator({message: 'Id is required.'}),
            new RangeValidator({min: 1, max: Number.MAX_VALUE, message: 'Id must be greater than 0.'})
        ],
        "Signal": [
            new RequiredValidator({message: 'Signal is required.'}),
            new EnumValidator({possibleValues: ['Cancel'], message: 'The only available signal is Cancel.'})
        ]
    }
});