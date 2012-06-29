/**
 * Implements the workers signal form.
 *
 * @constructor
 * @extends {FormView}
 */
var WorkersSignalView = FormView.extend({
    serializers: {
        "Id": new IntFieldSerializer()
    },
    template: _.template($('#workers-signal-template').html()),
    validators: {
        "Id": [
            new RequiredFieldValidator({message: 'Id is required.'}),
            new RangeFieldValidator({min: 1, max: Number.MAX_VALUE, message: 'Id must be greater than 0.'})
        ],
        "Signal": [
            new RequiredFieldValidator({message: 'Signal is required.'}),
            new EnumFieldValidator({possibleValues: ['Start', 'Stop'], message: 'Signal must be either Start or Stop.'})
        ]
    }
});