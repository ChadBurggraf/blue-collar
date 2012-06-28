var WorkersEditView = FormView.extend({
    serializers: {
        "Id": new IntFieldSerializer(),
        "QueueNames": new QueueNamesSerializer()
    },
    template: _.template($('#workers-edit-template').html()),
    validators: {
        "Name": [
            new RequiredFieldValidator({message: 'Name is required.'}),
            new LengthFieldValidator({maxLength: 64, message: 'Name cannot be longer than 64 characters.'})
        ],
        "MachineAddress": [
            new LengthFieldValidator({maxLength: 64, message: 'Machine address cannot be longer than 64 characters.'})
        ],
        "MachineName": [
            new LengthFieldValidator({maxLength: 128, message: 'Machine name cannot be longer than 128 characters.'})
        ],
        "Startup": [
            new RequiredFieldValidator({message: 'Startup is required.'}),
            new EnumFieldValidator({possibleValues: ['Automatic', 'Manual'], message: 'Startup must be either Automatic or Manual.'})
        ]
    }
});