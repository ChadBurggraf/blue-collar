/**
 * Implements the working signal form.
 *
 * @constructor
 * @extends {FormView}
 */
var WorkingSignalView = FormView.extend({
    serializers: {
        "Id": new IntFieldSerializer()
    },
    template: _.template($('#working-signal-template').html()),
    validators: {
        "Id": [
            new RequiredFieldValidator({message: 'Id is required.'}),
            new RangeFieldValidator({min: 1, max: Number.MAX_VALUE, message: 'Id must be greater than 0.'})
        ],
        "Signal": [
            new RequiredFieldValidator({message: 'Signal is required.'}),
            new EnumFieldValidator({possibleValues: ['Cancel'], message: 'Signal must be either Start or Stop.'})
        ]
    },

    /**
     * Gets the name of the action performed by this instance upon submission.
     *
     * @return {String} The name of the action performed by this instance.
     */
    getAction: function() {
        return 'signalled';
    }
});