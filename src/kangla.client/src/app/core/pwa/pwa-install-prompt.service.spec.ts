import { PwaInstallPromptService, PwaInstallPromptType } from './pwa-install-prompt.service';

interface FakeBrowser {
  document: Document;
  listeners: Map<string, EventListener>;
  storage: Map<string, string>;
}

function createFakeBrowser(options: { ios?: boolean; standalone?: boolean; storage?: Map<string, string> } = {}): FakeBrowser {
  const listeners = new Map<string, EventListener>();
  const storage = options.storage ?? new Map<string, string>();
  const localStorage: Storage = {
    getItem: key => storage.get(key) ?? null,
    setItem: (key, value) => storage.set(key, value),
    removeItem: key => storage.delete(key),
    clear: () => storage.clear(),
    key: index => [...storage.keys()][index] ?? null,
    get length() { return storage.size; }
  };
  const window = {
    addEventListener: (type: string, listener: EventListener) => listeners.set(type, listener),
    localStorage,
    matchMedia: () => ({ matches: options.standalone === true }),
    navigator: {
      userAgent: options.ios ? 'Mozilla/5.0 (iPhone; CPU iPhone OS 18_0 like Mac OS X)' : 'Mozilla/5.0 (Linux; Android 15)',
      platform: options.ios ? 'iPhone' : 'Linux armv8l',
      maxTouchPoints: options.ios ? 5 : 0
    }
  } as unknown as Window;

  return { document: { defaultView: window } as Document, listeners, storage };
}

function currentPrompt(service: PwaInstallPromptService): { value: PwaInstallPromptType | null } {
  const prompt = { value: null as PwaInstallPromptType | null };
  service.prompt$.subscribe(value => prompt.value = value);
  return prompt;
}

describe('PwaInstallPromptService', () => {
  it('shows the iOS installation guidance when not installed', () => {
    const browser = createFakeBrowser({ ios: true });
    const service = new PwaInstallPromptService(browser.document);

    expect(currentPrompt(service).value).toBe('ios');
  });

  it('does not show a prompt in standalone mode', () => {
    const browser = createFakeBrowser({ ios: true, standalone: true });
    const service = new PwaInstallPromptService(browser.document);

    expect(currentPrompt(service).value).toBeNull();
  });

  it('defers the native prompt until the user selects Install', async () => {
    const browser = createFakeBrowser();
    const service = new PwaInstallPromptService(browser.document);
    const prompt = currentPrompt(service);
    const nativePrompt = {
      preventDefault: jasmine.createSpy('preventDefault'),
      prompt: jasmine.createSpy('prompt').and.resolveTo(undefined),
      userChoice: Promise.resolve({ outcome: 'accepted' as const, platform: 'web' })
    };

    browser.listeners.get('beforeinstallprompt')?.(nativePrompt as unknown as Event);
    expect(nativePrompt.preventDefault).toHaveBeenCalled();
    expect(prompt.value).toBe('native');

    await service.install();

    expect(nativePrompt.prompt).toHaveBeenCalled();
    expect(prompt.value).toBeNull();
  });

  it('defers future prompts for seven days when the native dialog is dismissed', async () => {
    const storage = new Map<string, string>();
    const browser = createFakeBrowser({ storage });
    const service = new PwaInstallPromptService(browser.document);
    const nativePrompt = {
      preventDefault: () => {},
      prompt: () => Promise.resolve(),
      userChoice: Promise.resolve({ outcome: 'dismissed' as const, platform: 'web' })
    } as unknown as Event;

    browser.listeners.get('beforeinstallprompt')?.(nativePrompt);
    await service.install();

    const laterIosBrowser = createFakeBrowser({ ios: true, storage });
    const laterService = new PwaInstallPromptService(laterIosBrowser.document);
    expect(currentPrompt(laterService).value).toBeNull();
  });

  it('permanently suppresses the prompt until installation clears the preference', () => {
    const storage = new Map<string, string>();
    const browser = createFakeBrowser({ ios: true, storage });
    const service = new PwaInstallPromptService(browser.document);

    service.declinePermanently();
    const suppressedBrowser = createFakeBrowser({ ios: true, storage });
    expect(currentPrompt(new PwaInstallPromptService(suppressedBrowser.document)).value).toBeNull();

    browser.listeners.get('appinstalled')?.(new Event('appinstalled'));
    const resetBrowser = createFakeBrowser({ ios: true, storage });
    expect(currentPrompt(new PwaInstallPromptService(resetBrowser.document)).value).toBe('ios');
  });
});
