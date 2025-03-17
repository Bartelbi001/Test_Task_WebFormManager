<template>
  <div class="min-h-screen bg-gradient-to-r from-gray-100 to-gray-200 flex flex-col">
    <nav class="bg-gray-900 text-white p-4 flex justify-center gap-8 shadow-lg rounded-b-xl text-xl font-semibold">
      <router-link to="/main" class="hover:underline text-lg font-medium">About</router-link>
      <router-link to="/form" class="hover:underline text-lg font-medium">Form</router-link>
      <router-link to="/list" class="hover:underline text-lg font-medium">List</router-link>
    </nav>

    <div class="flex-1 flex justify-center items-start mt-8 px-4">
      <div class="w-full max-w-3xl p-8 bg-white shadow-lg rounded-xl">
        <router-view />
      </div>
    </div>

    <footer class="bg-gray-900 text-gray-300 text-center p-4 mt-6 rounded-t-xl">
      <p>© 2025 The test task for <a href="https://plumsail.com" class="text-blue-400 hover:underline">Plumsail</a></p>
    </footer>
  </div>
</template>