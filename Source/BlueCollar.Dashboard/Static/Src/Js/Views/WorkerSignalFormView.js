/**
 * Form view for signaling Worker models.
 */
var WorkerSignalFormView = FormView.extend({
    serializers: {
        "Id": new IntFieldSerializer()
    },

    template: _.template($('#worker-signal-form-template').html()),

    validators: {
        "Id": [
            new RequiredValidator({message: 'Id is required.'}),
            new RangeValidator({min: 1, max: Number.MAX_VALUE, message: 'Id must be greater than 0.'})
        ],
        "Signal": [
            new RequiredValidator({message: 'Signal is required.'}),
            new EnumValidator({possibleValues: ['Start', 'Stop'], message: 'Signal must be either Start or Stop.'})
        ]
    }
});