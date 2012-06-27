module('Validate');

test('ValidateRange', function() {
    equal(new RangeFormValidator({min: 0, max: 1}).validate(1), undefined);
    equal(new RangeFormValidator({min: 1, max: 2}).validate(1), undefined);
    equal(new RangeFormValidator({min: 0, max: 2}).validate(0), undefined);
    notEqual(new RangeFormValidator({min: 3, max: 4}).validate(2), undefined);
    equal(new RangeFormValidator({min: 0, max: 100}).validate(null), undefined);
    equal(new RangeFormValidator({min: Date.parse('2010-01-01'), max: Date.parse('2012-01-01')}).validate(Date.parse('2011-01-01')), undefined);
    notEqual(new RangeFormValidator({min: Date.parse('2011-02-01'), max: Date.parse('2012-01-01')}).validate(Date.parse('2011-01-01')), undefined);
});

test('ValidateRegex', function() {
    equal(new RegexFormValidator({exp: /^abc$/}).validate('abc'), undefined);
    equal(new RegexFormValidator({exp: '^abc$'}).validate('abc'), undefined);
    notEqual(new RegexFormValidator({exp: /^def$/}).validate('abc'), undefined);
    equal(new RegexFormValidator({exp: /^abc$/}).validate(''), undefined);
});

test('ValidateRequired', function() {
    equal(new RequiredFormValidator().validate('a'), undefined);
    notEqual(new RequiredFormValidator().validate(''), undefined);
    equal(new RequiredFormValidator().validate(0), undefined);
    equal(new RequiredFormValidator().validate(12), undefined);
    notEqual(new RequiredFormValidator().validate(NaN), undefined);
    equal(new RequiredFormValidator().validate(new Date()), undefined);
    equal(new RequiredFormValidator().validate([1, 2, 3]), undefined);
    notEqual(new RequiredFormValidator().validate([]), undefined);
    notEqual(new RequiredFormValidator().validate(undefined), undefined);
    notEqual(new RequiredFormValidator().validate(null), undefined);
    notEqual(new RequiredFormValidator().validate({}), undefined);
});