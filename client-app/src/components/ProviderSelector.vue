<template>
  <div class="provider-selector">
    <h2>Select Data Provider</h2>
    <button @click="selectProvider('LocalDb')">Use Local DB</button>
    <button @click="selectProvider('FirebaseRealtimeDb')">Use Cloud DB</button>
  </div>
</template>

<script>
export default {
  name: "ProviderSelector",
  methods: {
    selectProvider(provider) {
      this.$emit('selected', provider);
    }
  }
};
</script>

<style scoped>
.provider-selector button {
  margin: 5px;
  padding: 8px 12px;
  font-size: 14px;
  cursor: pointer;
  background-color: #3498db;
  color: white;
  border: none;
  border-radius: 5px;
}

.provider-selector button:hover {
  background-color: #2980b9;
}
</style>