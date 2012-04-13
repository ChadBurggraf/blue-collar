/**
 * Form view for Schedule models.
 */
var ScheduleFormView = FormView.extend({
    serializers: {
        "EndOn": new DateFieldSerializer(),
        "Id": new IntFieldSerializer(),
        "RepeatValue": new IntFieldSerializer(),
        "StartOn": new DateFieldSerializer()
    },

    template: _.template($('#schedule-form-template').html()),

    validators: {
        "QueueName": [
            new LengthValidator({maxLength: 24, message: 'Queue cannot be longer than 24 characters.'})
        ],
        "Name": [
            new RequiredValidator({message: 'Name is required.'}),
            new LengthValidator({maxLength: 24, message: 'Name cannot be longer than 24 characters.'})
        ],
        "StartOn": [
            new RequiredValidator({message: 'Start on is required.'}),
            new RangeValidator({min: Date.parse('1900-01-01'), max: Date.parse('2100-01-01'), message: 'Start on must be between 1900-01-01 and 2100-01-01.'})
        ],
        "EndOn": [
            new RangeValidator({min: Date.parse('1900-01-01'), max: Date.parse('2100-01-01'), message: 'End on must be between 1900-01-01 and 2100-01-01.'})
        ],
        "RepeatType": [
            new RequiredValidator({message: 'Repeat type is required.'}),
            new EnumValidator({possibleValues: ['None', 'Seconds', 'Minutes', 'Hours', 'Days', 'Weeks'], message: 'Repeat type must be one of None, Seconds, Minutes, Hours, Days or Weeks.'})
        ],
        "RepeatValue": [
            new RangeValidator({min: 1, max: Number.MAX_VALUE, message: 'Repeat value must be greater than 0.'})
        ]
    },

    focus: function() {
        this.$('input[name="Name"]').focus();
    },

    validate: function(obj) {
        var errors = FormView.prototype.validate.call(this, obj);

        if (_.isUndefined(errors)) {
            if (obj.RepeatType !== 'None' && (_.isUndefined(obj.RepeatValue) || _.isNull(obj.RepeatValue))) {
                errors = {
                    "RepeatValue": "Repeat value is required when repeat type is not 'None'."
                };
            }
        }

        return errors;
    }
});