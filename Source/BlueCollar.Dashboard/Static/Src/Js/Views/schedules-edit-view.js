/**
 * Implements the schedules edit form.
 *
 * @constructor
 * @extends {FormView}
 */
var SchedulesEditView = FormView.extend({
    serializers: {
        "EndOn": new DateFieldSerializer(),
        "Id": new IntFieldSerializer(),
        "RepeatValue": new IntFieldSerializer(),
        "StartOn": new DateFieldSerializer(),
        "Enabled": new BooleanFieldSerializer()
    },
    template: _.template($('#schedules-edit-template').html()),
    validators: {
        "QueueName": [
            new LengthFieldValidator({maxLength: 24, message: 'Queue cannot be longer than 24 characters.'})
        ],
        "Name": [
            new RequiredFieldValidator({message: 'Name is required.'}),
            new LengthFieldValidator({maxLength: 24, message: 'Name cannot be longer than 24 characters.'})
        ],
        "StartOn": [
            new RequiredFieldValidator({message: 'Start on is required.'}),
            new RangeFieldValidator({min: Date.parse('1900-01-01'), max: Date.parse('2100-01-01'), message: 'Start on must be between 1900-01-01 and 2100-01-01.'})
        ],
        "EndOn": [
            new RangeFieldValidator({min: Date.parse('1900-01-01'), max: Date.parse('2100-01-01'), message: 'End on must be between 1900-01-01 and 2100-01-01.'})
        ],
        "RepeatType": [
            new RequiredFieldValidator({message: 'Repeat type is required.'}),
            new EnumFieldValidator({possibleValues: ['None', 'Seconds', 'Minutes', 'Hours', 'Days', 'Weeks'], message: 'Repeat type must be one of None, Seconds, Minutes, Hours, Days or Weeks.'})
        ],
        "RepeatValue": [
            new RangeFieldValidator({min: 1, max: Number.MAX_VALUE, message: 'Repeat value must be greater than 0.'})
        ],
        "Enabled": [
            new RequiredFieldValidator({message: 'Enabled is required.'})
        ]
    },

    /**
     * Focuses the first element in the form.
     */
    focus: function() {
        this.$('input[name="Name"]').focus();
        return this;
    }
});