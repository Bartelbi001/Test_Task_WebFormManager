<template>
  <div class="min-h-screen flex items-center justify-center bg-gradient-to-r from-gray-100 to-gray-200 p-6">
    <div class="w-full max-w-md bg-white shadow-lg rounded-xl p-6">
      <h2 class="text-2xl font-bold text-gray-900 text-center mb-6">Sending Form</h2>

      <form @submit.prevent="handleSubmit" class="space-y-4">
        <!-- Name Field -->
        <div>
          <label class="block text-gray-700 font-medium mb-1">Name:</label>
          <Field
              name="name"
              type="text"
              v-model="formData.name"
              class="w-full p-2 border-2 border-blue-500 bg-blue-50 rounded-lg focus:outline-none
         focus:ring-2 focus:ring-blue-600 transition"
              placeholder="Enter your name..."
          />
          <ErrorMessage name="name" class="text-red-500 text-sm mt-1" />
        </div>

        <!-- Category Selection -->
        <div>
          <label class="block text-gray-700 font-medium mb-1">Select a category:</label>
          <Field
              as="select"
              name="category"
              v-model="formData.category"
              class="w-full p-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 transition"
          >
            <option value="">Choose...</option>
            <option value="option1">Option 1</option>
            <option value="option2">Option 2</option>
          </Field>
          <ErrorMessage name="category" class="text-red-500 text-sm mt-1" />
        </div>

        <!-- Date Field -->
        <Field
            name="birthdate"
            type="date"
            v-model="formData.birthdate"
            class="w-full p-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 transition"
            :max="new Date().toISOString().split('T')[0]"
            :min="minBirthDate"
            lang="en"
            placeholder="YYYY-MM-DD"
        />
        <ErrorMessage name="birthdate" class="text-red-500 text-sm mt-1" />

        <!-- Gender Radios -->
        <div>
          <label class="block text-gray-700 font-medium mb-1">Gender:</label>
          <div class="flex gap-4">
            <label class="flex items-center">
              <Field type="radio" name="gender" value="male" v-model="formData.gender" class="mr-2" />
              Male
            </label>
            <label class="flex items-center">
              <Field type="radio" name="gender" value="female" v-model="formData.gender" class="mr-2" />
              Female
            </label>
          </div>
          <ErrorMessage name="gender" class="text-red-500 text-sm mt-1" />
        </div>

        <!-- Terms Checkbox -->
        <div class="flex items-center gap-2">
          <Field type="checkbox" name="terms" v-model="formData.terms" class="h-5 w-5" />
          <label class="text-gray-700 font-medium">I agree with the terms</label>
        </div>
        <ErrorMessage name="terms" class="text-red-500 text-sm mt-1" />

        <!-- Message about successful sending -->
        <div v-if="successMessage" class="mt-4 p-3 bg-green-100 text-green-800 border border-green-300 rounded-lg text-center">
          ✅ {{ successMessage }}
        </div>

        <!-- Submit Button -->
        <button
            type="submit"
            class="w-full bg-gradient-to-r from-blue-200 to-blue-400 text-gray-900 px-4 py-2 rounded-lg font-semibold text-lg
         transition hover:from-blue-300 hover:to-blue-500 active:scale-95 focus:ring-4 focus:ring-blue-200 disabled:opacity-50"
            :disabled="isSubmitting"
        >
          Send
        </button>
      </form>
    </div>
  </div>
</template>

<script>
import { ref } from 'vue';
import { useForm, Field, ErrorMessage } from 'vee-validate';
import { formSchema } from '@/validation/formSchema';
import { submitForm } from '@/services/apiService';

export default {
  components: { Field, ErrorMessage },
  setup() {
    const formData = ref({
      name: '',
      category: '',
      birthdate: '',
      gender: '',
      terms: false,
    });

    const successMessage = ref('');
    const minBirthDate = ref(new Date(new Date().setFullYear(new Date().getFullYear() - 155)).toISOString().split('T')[0]);

    const { handleSubmit, isSubmitting, setErrors } = useForm({
      validationSchema: formSchema,
    });

    const submitData = async () => {
      try {
        await submitForm(formData.value);
        successMessage.value = "The form has been successfully submitted!";
        setTimeout(() => (successMessage.value = ''), 3000);
      } catch (error) {
        setErrors({ name: 'Error when sending data' });
      }
    };

    return { formData, handleSubmit: handleSubmit(submitData), isSubmitting, successMessage, minBirthDate };
  },
};
</script>
