import * as yup from 'yup';

const today = new Date();
const maxAge = new Date();
maxAge.setFullYear(today.getFullYear() - 155);

export const formSchema = yup.object({
    name: yup
        .string()
        .matches(/^[A-Za-zА-Яа-яЁё\s]+$/, 'Only letters and spaces')
        .required('The name is required')
        .trim(),
    category: yup.string().required('Select a category'),
    birthdate: yup
        .date()
        .max(today, 'The birth date cannot be in the future')
        .min(maxAge, 'The birth date must be within the last 155 years')
        .required('The date is required'),
    gender: yup.string().required('Choose a gender'),
    terms: yup.boolean().oneOf([true], 'The conditions must be accepted'),
});