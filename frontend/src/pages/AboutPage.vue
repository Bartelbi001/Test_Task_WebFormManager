<template>
  <div class="min-h-screen flex items-center justify-center bg-gradient-to-r from-gray-100 to-gray-200 p-6">
    <div class="max-w-2xl w-full bg-white shadow-xl rounded-2xl p-8">
      <!-- Title -->
      <h2 class="text-5xl font-extrabold text-gray-900 text-center uppercase tracking-wide underline shadow-md mb-6">
        About the Test Task
      </h2>

      <!-- Basic description -->
      <p class="text-lg text-gray-700 leading-relaxed text-left">
        This test task was completed for
        <a href="https://plumsail.com" class="text-blue-600 font-bold underline hover:text-blue-700 transition" target="_blank">
          Plumsail
        </a>.
        The project implements sending and storing dynamic forms with the ability to search and display data.
      </p>

      <!-- Technologies -->
      <div class="mt-6 p-6 bg-gray-100 rounded-2xl shadow-md text-left">
        <h3 class="text-xl font-semibold flex items-center gap-2 text-gray-900">
          🛠 <span>Technologies</span>
        </h3>
        <ul class="list-disc list-inside space-y-2 text-lg text-gray-800">
          <li><span class="text-indigo-600 font-bold">Frontend:</span> Vue 3 + Vite + Tailwind CSS</li>
          <li><span class="text-indigo-600 font-bold">Backend:</span> ASP.NET Core + In-Memory Storage</li>
          <li><span class="text-indigo-600 font-bold">API:</span> REST (Axios)</li>
        </ul>
      </div>

      <!-- How to check the project -->
      <div class="mt-6 p-6 bg-gray-100 rounded-2xl shadow-md text-left">
        <h3 class="text-xl font-semibold flex items-center gap-2 text-gray-900">
          🔍 <span>How to Check the Project?</span>
        </h3>
        <ul class="list-decimal list-inside space-y-2 text-lg text-gray-800">
          <li>Go to <router-link to="/form" class="text-blue-600 font-bold underline hover:text-blue-700">the form page</router-link> and send the data.</li>
          <li>Open <router-link to="/list" class="text-blue-600 font-bold underline hover:text-blue-700">list of applications</router-link> to see the results.</li>
          <li>Use the search to filter applications.</li>
        </ul>
      </div>

      <!-- Contacts -->
      <div class="mt-6 p-6 bg-gray-100 rounded-2xl shadow-md text-left">
        <h3 class="text-xl font-semibold flex items-center gap-2 text-gray-900">
          📩 <span>Contacts</span>
        </h3>
        <p class="text-lg text-gray-800">
          If you have any questions, you can contact me at
          <a href="http://shelkunov.20014@gmail.com/" class="text-blue-600 font-bold underline hover:text-blue-700 transition">
            email
          </a> or
          <a href="https://linkedin.com" class="text-blue-600 font-bold underline hover:text-blue-700 transition" target="_blank">
            LinkedIn
          </a>.
        </p>
      </div>
    </div>
  </div>
</template>
