import { fileURLToPath, URL } from 'node:url';
import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';
import { NodeGlobalsPolyfillPlugin } from '@esbuild-plugins/node-globals-polyfill';
import { NodeModulesPolyfillPlugin } from '@esbuild-plugins/node-modules-polyfill';
import { env } from 'process';

// Определяем целевой URL для прокси
const target = env.ASPNETCORE_HTTPS_PORT
    ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
    : env.ASPNETCORE_URLS
        ? env.ASPNETCORE_URLS.split(';')[0]
        : 'http://localhost:5210';

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [plugin()],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url)),
            buffer: 'buffer/' // Добавляем алиас для buffer
        }
    },
    server: {
        proxy: {
            '^/authorization': {
                target,
                secure: false
            },
            '^/listemployees': {
                target,
                secure: false
            },
            '^/employee': {
                target,
                secure: false
            },
            '^/analytics': {
                target,
                secure: false
            },
            '^/listsurveys': {
                target,
                secure: false
            },
            '^/survey': {
                target,
                secure: false
            },
            '^/surveycreator': {
                target,
                secure: false
            },
            '^/analyticsusersurvey': {
                target,
                secure: false
            },
            '^/listreports': {
                target,
                secure: false
            },
            '^/report': {
                target,
                secure: false
            },
        },
        port: 5173,
    },
    define: {
        global: {}, // Определяем глобальный объект
        'process.env': {} // Определяем переменные окружения
    },
    optimizeDeps: {
        esbuildOptions: {
            plugins: [
                NodeGlobalsPolyfillPlugin({ process: true, buffer: true }),
                NodeModulesPolyfillPlugin()
            ]
        }
    }
});