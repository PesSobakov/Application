import type { StorybookConfig } from '@storybook/angular';

const config: StorybookConfig = {
  "stories": [
    "../src/**/*.mdx",
    "../src/**/*.stories.@(js|jsx|mjs|ts|tsx)",
    "../src/app/**/*.stories.ts']",
  ],
  "addons": [
    "@storybook/addon-docs",
    "@storybook/addon-onboarding"
  ],
  webpackFinal: (config) => {
    config.module?.rules?.push({
      test: /\.css$/,
      exclude: /node_modules/,
      use: [
        {
          loader: 'postcss-loader',
          options: {
            postcssOptions: {
              plugins: ['@tailwindcss/postcss'],
            },
          },
        },
      ],
    });
    return config;
  },
  "framework": {
    "name": "@storybook/angular",
    "options": {}
  }
};
export default config;