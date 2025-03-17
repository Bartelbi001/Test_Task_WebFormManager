<template>
  <div class="min-h-screen flex items-center justify-center bg-gradient-to-r from-gray-100 to-gray-200 p-6">
    <div class="w-full max-w-2xl bg-white shadow-lg rounded-2xl p-6">
      <h2 class="text-2xl font-bold text-gray-900 text-center mb-4">📄 List of Applications</h2>

      <!-- Search field -->
      <div class="relative">
        <input
            type="text"
            v-model="searchQuery"
            @input="handleSearch"
            placeholder="🔍 Search..."
            class="w-full p-3 pr-10 border-2 border-gray-300 rounded-lg focus:outline-none
           focus:ring-2 focus:ring-blue-400 transition"
        />
        <div
            v-if="searchQuery"
            @click="clearSearch"
            class="absolute right-3 top-1/2 transform -translate-y-1/2 cursor-pointer text-gray-400 hover:text-gray-600"
        >
          ✖
        </div>
      </div>


      <!-- Loading indicator -->
      <div v-if="isLoading" class="flex justify-center py-6">
        <span class="animate-spin h-8 w-8 border-4 border-blue-500 border-t-transparent rounded-full"></span>
      </div>

      <!-- Ошибка загрузки -->
      <div v-if="errorMessage" class="text-red-500 text-center mt-4">{{ errorMessage }}</div>

      <!-- List of applications -->
      <ul v-if="submissions.length" class="mt-4 space-y-2">
        <TransitionGroup name="fade">
          <li
              v-for="submission in submissions"
              :key="submission.id"
              class="p-4 bg-gray-100 rounded-lg flex justify-between items-center shadow-sm hover:shadow-md transition"
          >
            <span class="font-semibold text-gray-800">{{ submission.name }}</span>
            <span class="text-gray-600 text-sm">({{ submission.category }})</span>
          </li>
        </TransitionGroup>
      </ul>

      <!-- If there are no applications -->
      <div v-else-if="!isLoading" class="text-gray-500 text-center mt-4">
        No applications found.
      </div>
    </div>
  </div>
</template>

<script>
import { ref, onMounted } from 'vue';
import { getSubmissions, searchSubmissions } from '@/services/apiService';

export default {
  setup() {
    const submissions = ref([]);
    const searchQuery = ref('');
    const isLoading = ref(false);
    const errorMessage = ref('');

    // Функция загрузки списка заявок
    const loadSubmissions = async () => {
      isLoading.value = true;
      errorMessage.value = '';
      try {
        submissions.value = await getSubmissions();
      } catch (error) {
        console.error('Error uploading applications:', error);
        errorMessage.value = 'Data loading error. Try again later.';
      } finally {
        isLoading.value = false;
      }
    };

    // Search with debounce
    let timeout;
    const handleSearch = () => {
      clearTimeout(timeout);
      timeout = setTimeout(async () => {
        if (searchQuery.value.trim()) {
          try {
            submissions.value = await searchSubmissions(searchQuery.value);
          } catch (error) {
            errorMessage.value = 'Search error. Try again later.';
          }
        } else {
          await loadSubmissions();
        }
      }, 400);
    };

    // Clearing the search
    const clearSearch = () => {
      searchQuery.value = '';
      loadSubmissions();
    };

    // Loading applications when mounting the component
    onMounted(loadSubmissions);

    return {
      submissions,
      searchQuery,
      isLoading,
      errorMessage,
      handleSearch,
      clearSearch,
    };
  },
};
</script>

<style>
/* Animation of the appearance of elements */
.fade-enter-active, .fade-leave-active {
  transition: opacity 0.3s ease-in-out, transform 0.3s ease-in-out;
}
.fade-enter-from, .fade-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}
</style>
